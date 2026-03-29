using AutoMapper;
using HRM_BE.Core.Data;
using HRM_BE.Core.Data.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.IRepositories;
using HRM_BE.Core.Models.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Data.SeedWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_BE.Data.Repositories
{
    public class TypeOfLeaveEmployeeRepository : RepositoryBase<TypeOfLeaveEmployee, int>, ITypeOfLeaveEmployeeRepository
    {

        private readonly IMapper _mapper;
        public TypeOfLeaveEmployeeRepository(HrmContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
            _mapper = mapper;
        }

       
        public async Task<TypeOfLeaveEmployeeDto> GetOrCreate(int? employeeId, int? typeOfLeaveId, int? year)
        {
            //_dbContext
            //throw new NotImplementedException();

            var query= _dbContext.TypeOfLeaveEmployees.Where(typeO=>typeO.IsDeleted!=true).AsQueryable();

            if (employeeId.HasValue)
            {
                query=query.Where(typeO=>typeO.EmployeeId==employeeId.Value);
            }

            if (typeOfLeaveId.HasValue)
            {
                query = query.Where(typeO => typeO.TypeOfLeaveId == typeOfLeaveId.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(typeO => typeO.Year == year.Value);
            }

            var typeOfLeaveEmployeeDto = await _mapper
                .ProjectTo<TypeOfLeaveEmployeeDto>(query.OrderByDescending(typeO => typeO.Id))
                .FirstOrDefaultAsync();

            if (typeOfLeaveEmployeeDto == null && employeeId.HasValue && typeOfLeaveId.HasValue && year.HasValue)
            {
                var typeOfLeave = _dbContext.TypeOfLeaves.Where(t => t.Id == typeOfLeaveId.Value).FirstOrDefault();
                var typeOfLeaveEmployee = new TypeOfLeaveEmployee();
                typeOfLeaveEmployee.EmployeeId = employeeId.Value;
                typeOfLeaveEmployee.TypeOfLeaveId = typeOfLeaveId.Value;
                typeOfLeaveEmployee.Year = year.Value;
                typeOfLeaveEmployee.DaysRemaining = typeOfLeave.MaximumNumberOfDayOff;

                await CreateAsync(typeOfLeaveEmployee);
                return _mapper.Map<TypeOfLeaveEmployeeDto>(typeOfLeaveEmployee);
            }

            return typeOfLeaveEmployeeDto;
        }

        public async Task<bool> UpdateDaysRemaining(double daysRemaining, int employeeId, int typeOfLeaveId, int year)
        {
            var typeOfLeaveEmployee = await _dbContext.TypeOfLeaveEmployees.Where(typeO => typeO.IsDeleted != true && typeO.EmployeeId == employeeId && typeO.TypeOfLeaveId == typeOfLeaveId && typeO.Year == year).FirstOrDefaultAsync();
            if (typeOfLeaveEmployee == null)
            {
                throw new EntityNotFoundException(nameof(typeOfLeaveEmployee), $"EmployeeId = {employeeId},TypeOfLeaveId={typeOfLeaveId},Year={year}");
            }
            typeOfLeaveEmployee.DaysRemaining= typeOfLeaveEmployee.DaysRemaining - daysRemaining;
            await UpdateAsync(typeOfLeaveEmployee);
            return true;
        }

        public async Task<bool> CheckDaysRemaining(double daysRemaining, int employeeId, int typeOfLeaveId, int year)
        {
            var typeOfLeaveEmployee = await _dbContext.TypeOfLeaveEmployees.Include(typeO=>typeO.TypeOfLeave).Where(typeO => typeO.IsDeleted != true && typeO.EmployeeId == employeeId && typeO.TypeOfLeaveId==typeOfLeaveId && typeO.Year==year).FirstOrDefaultAsync();
            if (typeOfLeaveEmployee==null)
            {
                return false;
            }
            else {
                //if(typeOfLeaveEmployee.TypeOfLeave == null)
                //{
                //    return false;
                //}
                //if (typeOfLeaveEmployee.TypeOfLeave.MaximumNumberOfDayOff < daysRemaining) { 
                //    return false;
                //}
                //return true;
                if (typeOfLeaveEmployee == null)
                {
                    return false;
                }
                if (typeOfLeaveEmployee.DaysRemaining < daysRemaining)
                {
                    return false;
                }
                return true;
            }

        }

    }
}
