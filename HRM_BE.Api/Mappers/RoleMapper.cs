using HRM_BE.Core.Data.Identity;
using HRM_BE.Core.Models.Identity.Permission;
using HRM_BE.Core.Models.Identity.Role;
using AutoMapper;

namespace HRM_BE.Api.Mappers
{
    public class RoleMapper : Profile
    {
        public RoleMapper()
        {

            CreateMap<Role, CreateRoleRequest>();
            CreateMap<CreateRoleRequest, Role>();
            CreateMap<EditRoleRequest, Role>();
            //CreateMap<Role, RoleDto>();
            CreateMap<Role, RoleDto>()
                // RolePermissions có thể không được Include khi gọi paging => cần null-safe để tránh NRE
                .ForMember(
                    dest => dest.Permissions,
                    opt => opt.MapFrom(src => src.RolePermissions == null
                        ? null
                        : src.RolePermissions.Select(rp => rp.Permission).ToList())
                );
            CreateMap<RoleDto, Role>();
        }
    }
}
