using AutoMapper;
using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.Models.Payroll_Timekeeping.Payroll;

namespace HRM_BE.Api.Mappings
{
    public class PayrollDetailProfile : Profile
    {
        public PayrollDetailProfile()
        {
            CreateMap<PayrollDetail, PayrollDetailEmailSendDto>();
        }
    }
}

