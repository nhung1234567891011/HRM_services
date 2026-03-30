/*
================================================================================
SCRIPT: RECALCULATE TIMESHEETS - HRM SYSTEM
================================================================================
Mục đích: Backup và verify dữ liệu trước khi recalculate NumberOfWorkingHour

LƯU Ý QUAN TRỌNG:
- Script này CHỈ để BACKUP và VERIFY dữ liệu
- KHÔNG NÊN chạy UPDATE trực tiếp bằng SQL vì logic phức tạp
- Sử dụng API endpoint: POST /api/time-sheet/recalculate-all
================================================================================
*/

-- 1. BACKUP dữ liệu hiện tại (chạy TRƯỚC KHI recalculate)
SELECT 
    Id,
    EmployeeId,
    ShiftWorkId,
    Date,
    StartTime,
    EndTime,
    NumberOfWorkingHour AS OldNumberOfWorkingHour,
    TimeKeepingLeaveStatus,
    LateDuration,
    EarlyLeaveDuration,
    CreatedAt,
    UpdatedAt
INTO Timesheet_Backup_Before_Recalculate
FROM Timesheets
WHERE IsDeleted = 0 OR IsDeleted IS NULL;

PRINT 'Backup completed. Total records: ' + CAST(@@ROWCOUNT AS VARCHAR(20));

-- 2. XEM dữ liệu có vấn đề (giờ làm âm hoặc > 24 giờ)
SELECT 
    t.Id,
    t.EmployeeId,
    e.FullName,
    t.Date,
    t.StartTime,
    t.EndTime,
    t.NumberOfWorkingHour,
    sc.WorkingHours AS StandardHours,
    CASE 
        WHEN t.NumberOfWorkingHour < 0 THEN 'Negative hours'
        WHEN t.NumberOfWorkingHour > 24 THEN 'Over 24 hours'
        WHEN t.NumberOfWorkingHour > (sc.WorkingHours + 4) THEN 'Suspicious overtime'
        WHEN t.StartTime IS NOT NULL AND t.EndTime IS NOT NULL 
             AND DATEDIFF(HOUR, t.StartTime, t.EndTime) != t.NumberOfWorkingHour THEN 'Mismatch calculation'
        ELSE 'OK'
    END AS IssueType
FROM Timesheets t
LEFT JOIN Employees e ON t.EmployeeId = e.Id
LEFT JOIN ShiftWorks sw ON t.ShiftWorkId = sw.Id
LEFT JOIN ShiftCatalogs sc ON sw.ShiftCatalogId = sc.Id
WHERE (t.IsDeleted = 0 OR t.IsDeleted IS NULL)
  AND (
    t.NumberOfWorkingHour < 0 
    OR t.NumberOfWorkingHour > 24
    OR (t.StartTime IS NOT NULL AND t.EndTime IS NOT NULL AND t.NumberOfWorkingHour IS NULL)
  )
ORDER BY t.Date DESC;

-- 3. THỐNG KÊ dữ liệu trước khi recalculate
SELECT 
    'Total records' AS Metric,
    COUNT(*) AS Count,
    AVG(NumberOfWorkingHour) AS AvgHours,
    MIN(NumberOfWorkingHour) AS MinHours,
    MAX(NumberOfWorkingHour) AS MaxHours
FROM Timesheets
WHERE (IsDeleted = 0 OR IsDeleted IS NULL)
  AND NumberOfWorkingHour IS NOT NULL

UNION ALL

SELECT 
    'Missing working hours' AS Metric,
    COUNT(*) AS Count,
    NULL AS AvgHours,
    NULL AS MinHours,
    NULL AS MaxHours
FROM Timesheets
WHERE (IsDeleted = 0 OR IsDeleted IS NULL)
  AND NumberOfWorkingHour IS NULL
  AND StartTime IS NOT NULL
  AND EndTime IS NOT NULL;

-- 4. VERIFY sau khi recalculate (chạy SAU KHI gọi API)
/*
SELECT 
    old.Id,
    old.EmployeeId,
    e.FullName,
    old.Date,
    old.OldNumberOfWorkingHour,
    new.NumberOfWorkingHour AS NewNumberOfWorkingHour,
    new.NumberOfWorkingHour - old.OldNumberOfWorkingHour AS Difference,
    CASE 
        WHEN ABS(new.NumberOfWorkingHour - old.OldNumberOfWorkingHour) > 0.01 THEN 'Changed'
        ELSE 'No change'
    END AS Status
FROM Timesheet_Backup_Before_Recalculate old
INNER JOIN Timesheets new ON old.Id = new.Id
INNER JOIN Employees e ON old.EmployeeId = e.Id
WHERE ABS(new.NumberOfWorkingHour - old.OldNumberOfWorkingHour) > 0.01
ORDER BY ABS(new.NumberOfWorkingHour - old.OldNumberOfWorkingHour) DESC;

PRINT 'Total changed records: ' + CAST(@@ROWCOUNT AS VARCHAR(20));
*/

-- 5. ROLLBACK nếu cần (chạy KHI có vấn đề)
/*
UPDATE t
SET 
    t.NumberOfWorkingHour = b.OldNumberOfWorkingHour,
    t.TimeKeepingLeaveStatus = b.TimeKeepingLeaveStatus
FROM Timesheets t
INNER JOIN Timesheet_Backup_Before_Recalculate b ON t.Id = b.Id;

PRINT 'Rollback completed. Total records: ' + CAST(@@ROWCOUNT AS VARCHAR(20));
*/

/*
================================================================================
HƯỚNG DẪN SỬ DỤNG:
================================================================================

BƯỚC 1: Backup dữ liệu
   - Chạy section 1 để backup

BƯỚC 2: Kiểm tra dữ liệu có vấn đề
   - Chạy section 2 và 3 để xem overview

BƯỚC 3: Recalculate bằng API
   - Gọi API: POST http://localhost:5000/api/time-sheet/recalculate-all
   - Hoặc với filter: POST http://localhost:5000/api/time-sheet/recalculate-all?startDate=2024-01-01&endDate=2024-12-31

BƯỚC 4: Verify kết quả
   - Chạy section 4 để so sánh trước/sau

BƯỚC 5: Rollback nếu cần (uncomment section 5)
   - CHỈ chạy khi có vấn đề nghiêm trọng

================================================================================
*/
