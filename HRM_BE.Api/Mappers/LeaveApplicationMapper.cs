using AutoMapper;
using HRM_BE.Core.Data.Official_Form;
using HRM_BE.Core.Data.Payroll_Timekeeping.LeaveRegulation;
using HRM_BE.Core.Data.Staff;
using HRM_BE.Core.Models.Official_Form.LeaveApplication;

namespace HRM_BE.Api.Mappers
{
    public class LeaveApplicationMapper:Profile
    {
        public LeaveApplicationMapper()
        {
            CreateMap<LeaveApplication, LeaveApplicationDto>().ReverseMap();
            CreateMap<LeaveApplication, UpdateStatusLeaveApplicationRequest>().ReverseMap();
            CreateMap<UpdateLeaveApplicationRequest, LeaveApplication>()
               .ForMember(dest => dest.LeaveApplicationApprovers, opt => opt.Ignore())
               .ForMember(dest => dest.LeaveApplicationReplacements, opt => opt.Ignore())
               .ForMember(dest => dest.LeaveApplicationRelatedPeople, opt => opt.Ignore())
               .ForMember(dest => dest.TypeOfLeave, opt => opt.Ignore())
               .ForMember(dest => dest.Employee, opt => opt.Ignore())
               .ForMember(dest => dest.Organization, opt => opt.Ignore())
               .ForMember(dest => dest.LeavePermission, opt => opt.Ignore());

            CreateMap<CreateLeaveApplicationRequest, LeaveApplication>()
           .ForMember(dest => dest.LeaveApplicationApprovers, opt => opt.MapFrom(src =>
               (src.ApproverIds ?? new List<int>())
                    .Distinct()
                    .Select(id => new LeaveApplicationApprover { ApproverId = id })))
           .ForMember(dest => dest.LeaveApplicationReplacements, opt => opt.MapFrom(src =>
               (src.ReplacementIds ?? new List<int>())
                    .Distinct()
                    .Select(id => new LeaveApplicationReplacement { ReplacementId = id })))
           .ForMember(dest => dest.LeaveApplicationRelatedPeople, opt => opt.MapFrom(src =>
               (src.RelatedPersonIds ?? new List<int>())
                    .Distinct()
                    .Select(id => new LeaveApplicationRelatedPerson { RelatedPersonId = id })));

            CreateMap<LeaveApplication, GetLeaveApplicationRequest>().ReverseMap();
            CreateMap<LeaveApplicationApprover, LeaveApplicationApproverDto>().ReverseMap();
            CreateMap<LeaveApplicationReplacement, LeaveApplicationReplacementDto>().ReverseMap();
            CreateMap<LeaveApplicationRelatedPerson, LeaveApplicationRelatedPersonDto>().ReverseMap();
            CreateMap<TypeOfLeave, LeaveApplicationTypeOfLeaveDto>().ReverseMap();

            CreateMap<StaffPosition, LeaveApplicationEmployeeStaffPositionDto>().ReverseMap();

        }
    }
}
