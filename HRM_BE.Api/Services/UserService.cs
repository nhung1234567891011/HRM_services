using HRM_BE.Api.Services;
using HRM_BE.Api.Services.Interfaces;
using HRM_BE.Core.Constants;
using HRM_BE.Core.Data.Identity;
using HRM_BE.Core.Exceptions;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Identity.Role;
using HRM_BE.Core.Models.Identity.User;
using HRM_BE.Core.Models.Mail;
using HRM_BE.Data;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using HRM_BE.Core.Models.Staff;
using System.Linq;


namespace HRM_BE.Api.Services
{
    public class UserService: IUserService
    {
        private readonly HrmContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly FileService _fileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;
        private MailService _mailService;

        public UserService(HrmContext dbContext, UserManager<User> userManager, FileService fileService, IHttpContextAccessor httpContextAccessor, IMapper mapper, RoleManager<Role> roleManager, MailService mailService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _roleManager = roleManager;
            _mailService = mailService;
        }

        
        public async Task<UserDto> GetById(EntityIdentityRequest<int> request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.Id.ToString());

                var userDto = _mapper.Map<UserDto>(user);

                return userDto;

            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCodeConstant.InternalServerError, ex);
            }
        }

        public async Task<UserDto> GetUserInfo(HttpContext httpContext)
        {
            try
            {
                var email = httpContext.User.Claims.First(x => x.Type == "Email").Value;

                var user = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    throw new ApiException("Không tìm thấy người dùng hợp lệ!", HttpStatusCodeConstant.BadRequest);
                }

                var permissionsTask = GetPermissionByUserIdAsync(user.Id);
                var rolesTask = GetRoleNormalizedAsync(user.Id);
                var employeeTask = GetEmployeeByUser(user.EmployeeId);

                await Task.WhenAll(permissionsTask, rolesTask, employeeTask);

                var permissions = await permissionsTask;
                var roles = await rolesTask;
                var employee = await employeeTask;

                var company = new UserCompanyDto();
                var organization = new UserOrganizationDto();

                if (employee != null)
                {
                    var companyTask = GetCompanyByEmployee(employee.CompanyId);
                    var organizationTask =
                        GetOrganizationByEmployee(employee.OrganizationId);

                    await Task.WhenAll(companyTask, organizationTask);

                    company = await companyTask;
                    organization = await organizationTask;
                }

                var userDto = _mapper.Map<UserDto>(user);


                userDto.Permissions = permissions;

                userDto.RoleNames = roles;

                userDto.Employee= employee;

                userDto.Company= company;

                userDto.Organization= organization;


                return userDto;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCodeConstant.InternalServerError, ex);
            }
        }

        public async Task<UserDto?> GetUserInfoAsync()
        {
            try
            {
                var email = _httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == "Email").Value;

                var user = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    throw new ApiException("Không tìm thấy người dùng hợp lệ!", HttpStatusCodeConstant.BadRequest);
                }

                var permissionsTask = GetPermissionByUserIdAsync(user.Id);
                var rolesTask = GetRoleNormalizedAsync(user.Id);
                var employeeTask = GetEmployeeByUser(user.EmployeeId);

                await Task.WhenAll(permissionsTask, rolesTask, employeeTask);

                var permissions = await permissionsTask;
                var roles = await rolesTask;
                var employee = await employeeTask;

                var company = new UserCompanyDto();
                var organization = new UserOrganizationDto();


                if (employee != null)
                {
                    var companyTask = GetCompanyByEmployee(employee.CompanyId);
                    var organizationTask =
                        GetOrganizationByEmployee(employee.OrganizationId);

                    await Task.WhenAll(companyTask, organizationTask);

                    company = await companyTask;
                    organization = await organizationTask;

                }

                var userDto = _mapper.Map<UserDto>(user);


                userDto.Permissions = permissions;

                userDto.RoleNames = roles;

                userDto.Employee = employee;

                userDto.Company = company;

                userDto.Organization = organization;



                return userDto;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, HttpStatusCodeConstant.InternalServerError, ex);
            }
        }

        public async Task<User> EditUserInfo(EditUserInfoRequest request)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _dbContext.Users.FindAsync(request.Id);

                    if (user == null)
                    {
                        throw new ApiException("Không tìm thấy user có Id hợp lệ!", HttpStatusCodeConstant.BadRequest);
                    }

                    _mapper.Map(request, user);

                    await _dbContext.SaveChangesAsync();


                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return user;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    throw new ApiException("Có lỗi xảy ra trong quá trình xử lý!", HttpStatusCodeConstant.InternalServerError, ex);
                }
            }
        }

        public async Task<UserDto> AssignUserToRolesAsync(AssignUserToRoleRequest request)
        {
            if (request.RoleNames == null || !request.RoleNames.Any())
            {
                //throw new ApiException("Danh sách vai trò không hợp lệ!", HttpStatusCodeConstant.BadRequest);
            }

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new ApiException("Không tìm thấy user!", HttpStatusCodeConstant.NotFound);
            }

            try
            {
                var userRoles = _dbContext.UserRoles.Where(ur => ur.UserId == user.Id);
                _dbContext.UserRoles.RemoveRange(userRoles);
                await _dbContext.SaveChangesAsync();


                var addResult = await _userManager.AddToRolesAsync(user, request.RoleNames);
                if (!addResult.Succeeded)
                {
                    var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                    throw new Exception($"Có lỗi xảy ra khi thêm vai trò: {errors}");
                }
                if (!addResult.Succeeded)
                {
                    var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                    throw new Exception($"Có lỗi xảy ra khi thêm vai trò: {errors}");
                }

                user.IsRefreshToken = true;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    throw new Exception($"Có lỗi xảy ra khi cập nhật user: {errors}");
                }

                var userDto = _mapper.Map<UserDto>(user);
                return userDto;
            }
            catch (Exception ex)
            {
                throw new ApiException($"Có lỗi xảy ra khi gán vai trò cho user: {ex.Message}", HttpStatusCodeConstant.InternalServerError);
            }
        }
        public async Task<UserDto> AssignEmployeeToRolesAsync(AssignEmployeeToRoleRequest request)
        {
            if (request.RoleNames == null || !request.RoleNames.Any())
            {
                //throw new ApiException("Danh sách vai trò không hợp lệ!", HttpStatusCodeConstant.BadRequest);
            }


            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.EmployeeId == request.EmployeeId);
            if (user == null)
            {
                throw new ApiException("Không tìm thấy user!", HttpStatusCodeConstant.NotFound);
            }

            try
            {
                var userRoles = _dbContext.UserRoles.Where(ur => ur.UserId == user.Id);
                _dbContext.UserRoles.RemoveRange(userRoles);
                await _dbContext.SaveChangesAsync();


                var addResult = await _userManager.AddToRolesAsync(user, request.RoleNames);
                if (!addResult.Succeeded)
                {
                    var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                    throw new Exception($"Có lỗi xảy ra khi thêm vai trò: {errors}");
                }
                if (!addResult.Succeeded)
                {
                    var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                    throw new Exception($"Có lỗi xảy ra khi thêm vai trò: {errors}");
                }

                user.IsRefreshToken = true;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    throw new Exception($"Có lỗi xảy ra khi cập nhật user: {errors}");
                }

                var userDto = _mapper.Map<UserDto>(user);
                return userDto;
            }
            catch (Exception ex)
            {
                throw new ApiException($"Có lỗi xảy ra khi gán vai trò cho user: {ex.Message}", HttpStatusCodeConstant.InternalServerError);
            }
        }

        //public async Task<UserDto> AssignEmployeeToRolesAsync(AssignEmployeeToRoleRequest request)
        //{
        //    if (request.RoleNames == null || !request.RoleNames.Any())
        //    {
        //        throw new ApiException("Danh sách vai trò không hợp lệ!", HttpStatusCodeConstant.BadRequest);
        //    }

        //    var user = await _userManager.Users.FirstOrDefaultAsync(u => u.EmployeeId == request.EmployeeId);
        //    if (user == null)
        //    {
        //        throw new ApiException("Không tìm thấy user!", HttpStatusCodeConstant.NotFound);
        //    }

        //    try
        //    {
        //        // Lấy danh sách vai trò hiện tại của người dùng
        //        var userRoles = await _userManager.GetRolesAsync(user);

        //        // Lọc ra các vai trò mà người dùng chưa có
        //        var rolesToAdd = request.RoleNames.Except(userRoles).ToList();

        //        if (rolesToAdd.Any())
        //        {
        //            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
        //            if (!addResult.Succeeded)
        //            {
        //                var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
        //                throw new ApiException($"Có lỗi xảy ra khi thêm vai trò: {errors}", HttpStatusCodeConstant.InternalServerError);
        //            }
        //        }

        //        user.IsRefreshToken = true;
        //        var updateResult = await _userManager.UpdateAsync(user);
        //        if (!updateResult.Succeeded)
        //        {
        //            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
        //            throw new ApiException($"Có lỗi xảy ra khi cập nhật user: {errors}", HttpStatusCodeConstant.InternalServerError);
        //        }

        //        var userDto = _mapper.Map<UserDto>(user);
        //        return userDto;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApiException($"Có lỗi xảy ra khi gán vai trò cho user: {ex.Message}", HttpStatusCodeConstant.InternalServerError);
        //    }
        //}

        //public async Task<IdentityResult> AddToRolesAsync(IdentityUser user, IEnumerable<string> roles)
        //{
        //    if (user == null) throw new ArgumentNullException(nameof(user));
        //    if (roles == null) throw new ArgumentNullException(nameof(roles));

        //    // Đảm bảo tất cả vai trò đều tồn tại
        //    var roleEntities = await _roleManager.Roles.Where(r => roles.Contains(r.Name)).ToListAsync();
        //    if (roleEntities.Count != roles.Count())
        //    {
        //        return IdentityResult.Failed(new IdentityError { Description = "One or more roles do not exist." });
        //    }

        //    // Lấy danh sách các vai trò hiện tại của người dùng
        //    var currentRoles = await _dbContext.UserRoles
        //        .Where(ur => ur.UserId.ToString() == user.Id)
        //        .Select(ur => ur.RoleId)
        //        .ToListAsync();

        //    // Tìm các vai trò mới để thêm vào

        //    var newRoles = roleEntities.Where(r => !currentRoles.Contains(r.Id)).ToList();
        //    foreach (var role in newRoles)
        //    {
        //        _dbContext.UserRoles.Add(new IdentityUserRole<int> { UserId = (int)user.Id, RoleId = role.Id });
        //    }

        //    try
        //    {
        //        await _dbContext.SaveChangesAsync();
        //        return IdentityResult.Success;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log and handle exceptions appropriately
        //        return IdentityResult.Failed(new IdentityError { Description = $"Error adding user to roles: {ex.Message}" });
        //    }
        //}


        public async Task<List<string>> GetRoleByUserAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return roles.ToList();
        }


        private async Task<List<string>> GetRoleNormalizedAsync(int userId)
        {
            return await _dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .Join(
                    _dbContext.Roles.AsNoTracking(),
                    ur => ur.RoleId,
                    r => r.Id,
                    (_, r) => r.NormalizedName)
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToListAsync();
        }



        public async Task<List<string>> GetPermissionByUserAsync(User user)
        {
            return await GetPermissionByUserIdAsync(user.Id);
        }

        private async Task<List<string>> GetPermissionByUserIdAsync(int userId)
        {
            return await _dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .Join(
                    _dbContext.RolePermissions.AsNoTracking(),
                    ur => ur.RoleId,
                    rp => rp.RoleId,
                    (_, rp) => rp.PermissionId)
                .Join(
                    _dbContext.Permissions.AsNoTracking(),
                    permissionId => permissionId,
                    p => p.Id,
                    (_, p) => p.Name)
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToListAsync();
        }

        private async Task<List<string>> GetUserWithRolePermission(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var permissions = await _dbContext.Roles.Where(role => roles.Contains(role.Name))
            .SelectMany(role => role.RolePermissions)
            .Select(rolePermission => rolePermission.Permission.Name).ToListAsync();

            return permissions;
        }


        public async Task<ConfirmEmailResult> VerifyEmailWithOtp(string? email, string otp)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new ConfirmEmailResult
                {
                    Success = false,
                    ErrorMessage = "Không tồn tại người dùng trong hệ thống"
                };
            }

            var otpValid = await _userManager.VerifyChangePhoneNumberTokenAsync(user, otp, user.PhoneNumber);

            if (!otpValid)
            {

                return new ConfirmEmailResult
                {
                    Success = false,
                    ErrorMessage = "OTP đã hết hạn. Vui lòng yêu cầu mã OTP mới"
                };

            }

            return new ConfirmEmailResult
            {
                Success = true
            };
        }


        private async Task<UserEmployeeDto> GetEmployeeByUser(int? employeeId)
        {
            if (employeeId == null)
            {
                return null;
            }
            else
            {
                var employee = await _dbContext.Employees.AsNoTracking().Where(x => x.Id == employeeId).Include(x=>x.StaffPosition).FirstOrDefaultAsync();
                if (employee == null)
                {
                    return null;
                }
                return _mapper.Map<UserEmployeeDto>(employee);
            }
        }

        private async Task<UserCompanyDto> GetCompanyByEmployee(int? companyId)
        {
            if (companyId == null) {
                return null;
            }
            else
            {
                var company = await _dbContext.Companies.AsNoTracking().Where(x=>x.Id == companyId).FirstOrDefaultAsync();
                if (company == null) {
                    return null;
                }
                return _mapper.Map<UserCompanyDto>(company);
            }
        }

        private async Task<UserOrganizationDto> GetOrganizationByEmployee(int? organizationId)
        {
            if (organizationId == null)
            {
                return null;
            }
            else
            {
                var organization = await _dbContext.Organizations.AsNoTracking().Where(x => x.Id == organizationId).FirstOrDefaultAsync();
                if (organization == null)
                {
                    return null;
                }
                return _mapper.Map<UserOrganizationDto>(organization);
            }
        }

    }
}
