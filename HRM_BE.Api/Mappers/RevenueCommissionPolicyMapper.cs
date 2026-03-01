using AutoMapper;
using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.Models.Payroll_Timekeeping.Payroll;

namespace HRM_BE.Api.Mappers
{
    public class RevenueCommissionPolicyMapper : Profile
    {
        public RevenueCommissionPolicyMapper()
        {
            CreateMap<RevenueCommissionTier, RevenueCommissionTierDto>().ReverseMap();

            CreateMap<RevenueCommissionPolicy, RevenueCommissionPolicyDto>()
                .ForMember(dest => dest.OrganizationName,
                    opt => opt.MapFrom(src => src.Organization != null ? src.Organization.OrganizationName : null))
                .ForMember(dest => dest.Tiers,
                    opt => opt.MapFrom(src => src.Tiers.OrderBy(t => t.SortOrder)))
                .ReverseMap();
        }
    }
}

