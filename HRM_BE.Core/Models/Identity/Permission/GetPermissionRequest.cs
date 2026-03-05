
using HRM_BE.Core.Data.Identity;
using HRM_BE.Core.Models.Common;

namespace HRM_BE.Core.Models.Identity.Permission
{
    public class GetPermissionRequest:PagingRequest
    {
        public string? Keyword { get; set; }
        public Section? Section { get; set; }
    }
}
