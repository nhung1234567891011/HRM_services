using AutoMapper;
using HRM_BE.Core.Data.Official_Form;
using HRM_BE.Core.Models.Official_Form.CheckInCheckOut;

namespace HRM_BE.Api.Mappers
{
    public class CheckInCheckOutApplicationMapper : Profile
    {
        public CheckInCheckOutApplicationMapper()
        {
            CreateMap<CheckInCheckOutApplication, CheckInCheckOutApplicationDto>().ReverseMap();
        }
    }
}

