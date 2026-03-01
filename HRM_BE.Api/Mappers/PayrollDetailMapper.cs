using System.Linq;
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
                .ForMember(dest => dest.SocialInsurance, opt => opt.MapFrom(src => src.BhxhAmount))
                .ForMember(dest => dest.TotalAllowance, opt => opt.MapFrom(src => (src.AllowanceMealTravel ?? 0) + (src.ParkingAmount ?? 0)))
                .ForMember(dest => dest.UnionFee, opt => opt.MapFrom(src => src.UnionFeeAmount))
                .ForMember(dest => dest.TotalDeduction, opt => opt.MapFrom(src =>
                    src.Employee != null && src.Employee.Deductions != null
                        ? src.Employee.Deductions.Where(d => d.IsDeleted != true).Sum(d => d.Value ?? 0)
                        : 0m))
                .ReverseMap();
        }
    }
}
