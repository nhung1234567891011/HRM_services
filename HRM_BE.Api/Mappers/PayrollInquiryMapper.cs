using HRM_BE.Core.Data.Payroll_Timekeeping.Payroll;
using HRM_BE.Core.Models.Payroll_Timekeeping.Payroll;

namespace HRM_BE.Api.Mappers
{
    public class PayrollInquiryMapper : AutoMapper.Profile
    {
        public PayrollInquiryMapper()
        {
            CreateMap<CreatePayrollInquiryRequest, PayrollInquiry>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content != null ? src.Content.Trim() : src.Content))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => InquiryStatus.Pending));

            CreateMap<PayrollInquiry, PayrollInquiryDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.PayrollDetail.FullName))
                .ForMember(dest => dest.TimeSent, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.PayrollDetail.EmployeeId))
                .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(src => src.PayrollDetail.EmployeeCode))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.PayrollDetail.Department))
                .ForMember(dest => dest.PayrollName, opt => opt.MapFrom(src => src.PayrollDetail.Payroll.PayrollName));
        }
    }
}
