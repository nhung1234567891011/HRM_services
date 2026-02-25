using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.Models.Payroll_Timekeeping.Payroll;

namespace HRM_BE.Api.Mappers
{
    public class PayrollDetailMapper : AutoMapper.Profile
    {
        public PayrollDetailMapper()
        {
            CreateMap<PayrollDetail, PayrollDetailDto>()
                .ForMember(dest => dest.OrganizationName,
                    opt => opt.MapFrom(src => src.Organization != null ? src.Organization.OrganizationName : null))
                .ForMember(dest => dest.Deductions,
                    opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Deductions : null))
                .ReverseMap();
        }
    }
}
