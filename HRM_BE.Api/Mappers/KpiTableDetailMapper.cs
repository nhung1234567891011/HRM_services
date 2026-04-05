using AutoMapper;
using HRM_BE.Core.Data.Salary;
using HRM_BE.Core.Models.Salary.KpiTable;
using HRM_BE.Core.Models.Salary.KpiTableDetail;

namespace HRM_BE.Api.Mappers
{
    public class KpiTableDetailMapper : Profile
    {
        public KpiTableDetailMapper()
        {
            CreateMap<CreateKpiTableDetailRequest, KpiTableDetail>();
            CreateMap<UpdateKpiTableDetailRequest, KpiTableDetail>();
            CreateMap<KpiTableDetail, KpiTableDetailDto>()
                .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.EmployeeCode : src.EmployeeCode))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? (src.Employee.LastName + " " + src.Employee.FirstName) : src.EmployeeName))
                .ForMember(dest => dest.StaffPositionCode, opt => opt.MapFrom(src =>
                    src.Employee != null && src.Employee.StaffPosition != null ? src.Employee.StaffPosition.PositionCode : null))
                .ForMember(dest => dest.IsRevenueEditable, opt => opt.MapFrom(src =>
                    src.Employee != null
                    && src.Employee.StaffPosition != null
                    && (
                        (src.Employee.StaffPosition.PositionCode != null
                         && (src.Employee.StaffPosition.PositionCode.ToUpper().StartsWith("SALE")
                             || src.Employee.StaffPosition.PositionCode.ToUpper() == "CTV"))
                        || (src.Employee.StaffPosition.PositionName != null
                            && (src.Employee.StaffPosition.PositionName.ToUpper().Contains("SALE")
                                || src.Employee.StaffPosition.PositionName.ToUpper().Contains("CTV")))
                    )))
                .ReverseMap();
        }
    }
}
