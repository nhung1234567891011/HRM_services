# ⚡ QUICK START - HỆ THỐNG TÍNH CHẤM CÔNG MỚI

## 🎯 TÓM TẮT

Backend giờ đây **TỰ ĐỘNG TÍNH** `NumberOfWorkingHour` theo nghiệp vụ chuẩn HRM:
- ✅ Cap theo giờ ca làm việc
- ✅ Trừ nghỉ giữa ca (lunch break)
- ✅ Xử lý check-in sớm / check-out muộn
- ✅ Xử lý thiếu check-in hoặc check-out
- ❌ KHÔNG TIN dữ liệu từ Frontend

---

## 🚀 CÁCH SỬ DỤNG

### Frontend (QUAN TRỌNG!)

**❌ TRƯỚC ĐÂY (SAI):**
```typescript
const request = {
  employeeId: 1,
  shiftWorkId: 5,
  startTime: "08:00:00",
  endTime: "17:00:00",
  numberOfWorkingHour: 8.0  // ← XÓA FIELD NÀY!
}
```

**✅ BÂY GIỜ (ĐÚNG):**
```typescript
const request = {
  employeeId: 1,
  shiftWorkId: 5,
  startTime: "08:00:00",
  endTime: "17:00:00"
  // Backend sẽ tự tính numberOfWorkingHour
}
```

### Backend (Tự Động)

Tất cả API sau sẽ TỰ ĐỘNG tính:
- `POST /api/time-sheet/create`
- `PUT /api/time-sheet/update`
- `POST /api/checkin-checkout-application/...`

---

## 🔄 RECALCULATE DATABASE (Chạy 1 Lần)

Sau khi deploy code mới, BẮT BUỘC phải recalculate database cũ:

```bash
# 1. Backup trước (QUAN TRỌNG!)
# Chạy SQL: Scripts/RecalculateTimesheets.sql (Section 1)

# 2. Recalculate
curl -X POST http://localhost:5000/api/time-sheet/recalculate-all \
  -H "Authorization: Bearer YOUR_TOKEN"

# 3. Verify
# Chạy SQL: Scripts/RecalculateTimesheets.sql (Section 4)
```

---

## 📖 VÍ DỤ NHANH

### Ví Dụ 1: Check-in đúng giờ
```
Ca: 08:00 - 17:15
Check-in: 08:00
Check-out: 17:15

→ NumberOfWorkingHour = 8h (đã trừ 1.25h nghỉ trưa)
```

### Ví Dụ 2: Check-in muộn
```
Ca: 08:00 - 17:15
Check-in: 09:30
Check-out: 17:15

→ NumberOfWorkingHour = 6.5h (đã trừ nghỉ trưa)
```

### Ví Dụ 3: Thiếu check-out
```
Ca: 08:00 - 17:15
Check-in: 08:00
Check-out: NULL

→ NumberOfWorkingHour = 0h
```

---

## 📚 CHI TIẾT

Xem file sau để hiểu rõ hơn:
- `TIMESHEET_CALCULATION_GUIDE.md` - Hướng dẫn đầy đủ
- `REFACTOR_SUMMARY.md` - Tổng kết kỹ thuật chi tiết
- `Scripts/RecalculateTimesheets.sql` - SQL scripts

---

## ⚠️ CHECKLIST DEPLOY

- [ ] Build thành công (`dotnet build`)
- [ ] Backup database
- [ ] Deploy code mới
- [ ] **Recalculate database** (POST /api/time-sheet/recalculate-all)
- [ ] Verify kết quả (SQL script)
- [ ] Update Frontend (xóa numberOfWorkingHour)
- [ ] Test end-to-end
- [ ] Monitor logs 24h

---

**Status:** ✅ READY TO DEPLOY  
**Build:** ✅ SUCCESS  
**Tests:** Pending manual verification
