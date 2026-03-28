using HRM_BE.Api.Services;
using HRM_BE.Api.Services.Interfaces;
using HRM_BE.Core.Constants;
using HRM_BE.Core.Constants.Identity;
using HRM_BE.Core.Data.Identity;
using HRM_BE.Core.Models.Auth;
using HRM_BE.Core.Models.Common;
using HRM_BE.Core.Models.Identity.User;
using HRM_BE.Core.Models.Mail;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;
using System.Text;
using HRM_BE.Core.Data.Staff;

namespace HRM_BE.Api.Controllers.Identity
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const string ActivationCompanyName = "CÔNG TY TNHH XUẤT NHẬP KHẨU HỮU CƠ VIỆT NAM EUCHOICE";

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config, IAuthService authService, IUserService userService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _authService = authService;
            _userService = userService;
            _mapper = mapper;
        }

        private static string BuildActivationLink(string urlClient, string email, string activationCode)
        {
            return $"{urlClient}/auth/set-password?email={Uri.EscapeDataString(email)}&activationCode={Uri.EscapeDataString(activationCode)}";
        }

        private static string BuildActivationMailSubject()
        {
            return $"Kích hoạt tài khoản hệ thống HRM - {ActivationCompanyName}";
        }

        private static string BuildActivationMailBody(User user, string activationLink)
        {
            return $@"
                <div style='margin: 0 auto; max-width: 640px; background: #ffffff; border: 1px solid #dbe4ef; border-radius: 14px; overflow: hidden; font-family: Tahoma, Arial, sans-serif; color: #1f2937;'>
                    <div style='padding: 22px 24px; background: linear-gradient(135deg, #14532d 0%, #166534 55%, #15803d 100%);'>
                        <h2 style='margin: 0; color: #ffffff; font-size: 21px; font-weight: 700;'>Kích hoạt tài khoản HRM</h2>
                        <p style='margin: 8px 0 0 0; color: #dcfce7; font-size: 13px;'>{ActivationCompanyName}</p>
                    </div>

                    <div style='padding: 24px;'>
                        <p style='margin: 0 0 12px 0; font-size: 16px; font-weight: 600;'>Kính gửi: Anh/Chị {user.Name},</p>
                        <p style='margin: 0 0 16px 0; font-size: 14px; line-height: 1.7;'>
                            Tài khoản của Anh/Chị đã được tạo trên hệ thống HRM. Vui lòng nhấn nút bên dưới để thiết lập mật khẩu và kích hoạt tài khoản.
                        </p>

                        <div style='background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 10px; padding: 14px 16px; margin-bottom: 20px;'>
                            <p style='margin: 0; font-size: 13px; color: #334155;'><strong>Tài khoản đăng nhập:</strong> {user.Email}</p>
                            <p style='margin: 8px 0 0 0; font-size: 13px; color: #b45309;'>Lưu ý: Liên kết kích hoạt có hiệu lực trong 24 giờ.</p>
                        </div>

                        <div style='text-align: center; margin: 22px 0;'>
                            <a href='{activationLink}' target='_blank' style='display: inline-block; padding: 12px 30px; border-radius: 999px; background: linear-gradient(135deg, #14532d 0%, #15803d 100%); color: #ffffff; text-decoration: none; font-size: 15px; font-weight: 700;'>
                                Kích hoạt ngay
                            </a>
                        </div>

                        <p style='margin: 0; font-size: 13px; color: #64748b;'>Nếu Anh/Chị không thực hiện yêu cầu này, vui lòng bỏ qua email.</p>
                        <p style='margin: 16px 0 0 0; font-size: 14px;'>Trân trọng,<br><strong>{ActivationCompanyName}</strong></p>
                    </div>
                </div>
            ";
        }

        [HttpPost]
        [Route("login-by-email")]
        public async Task<ApiResult<LoginResult>> LoginByEmail([FromBody] LoginEmailRequest request)
        {
            //Check exists user
            var user = _userManager.Users.Include(u => u.Employee).FirstOrDefault(user => user.Email == request.Email && user.EmailConfirmed == true);

            if (user == null)
            {
                return new ApiResult<LoginResult>()
                {
                    Status = false,
                    Message = "Email người dùng không tồn tại!",
                    Data = null
                };
            }
            if (user.IsActivated == false)
            {
                return new ApiResult<LoginResult>()
                {
                    Status = false,
                    Message = "Tài khoản chưa kích hoạt",
                    Data = null
                };
            }
            if (user.IsLockAccount == true)
            {
                return new ApiResult<LoginResult>()
                {
                    Status = false,
                    Message = "Tài khoản đã bị khóa!",
                    Data = null
                };
            }
            //Verify login
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);

            if (!result.Succeeded)
            {
                return new ApiResult<LoginResult>()
                {
                    Status = false,
                    Message = "Tài khoản hoặc mật khẩu người dùng không chính xác!",
                    Data = null
                };
            }

            //Create token
            var token = await _authService.CreateToken(user);
            var refreshToken = _authService.CreateRefreshToken();

            var refreshTokenValidityInDays = _config["JwtTokenSettings:RefreshTokenValidityInDays"];

            if (string.IsNullOrEmpty(refreshTokenValidityInDays))
            {
                throw new ArgumentNullException(nameof(refreshTokenValidityInDays), "Không thể tải cấu hình RefreshTokenValidityInDays Jwt!");
            }

            var refreshTokenExpiryTime = DateTime.Now.AddDays(int.Parse(refreshTokenValidityInDays));

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiryTime;

            await _userManager.UpdateAsync(user);


            var loginResult = new LoginResult()
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                Expiration = DateTime.Now.AddDays(30)
            };


            return new ApiResult<LoginResult>()
            {
                Status = true,
                Message = "Đăng nhập thành công!",
                Data = loginResult
            };


        }

        [HttpPost("register-verify-by-email")]
        public async Task<ApiResult<bool>> RegisterVerifyByEmail([FromBody] CreateUserRequest request)
        {
            var phoneNumber = request?.PhoneNumber?.Trim();
            var email = request?.Email?.Trim();

            var user = await _userManager.Users
                          .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber || u.Email == email);

            if (user != null && user.EmailConfirmed)
            {
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = user.PhoneNumber == phoneNumber ?
                              "Số điện thoại đã tồn tại trong hệ thống" : "Email đã tồn tại trong hệ thống",
                    Data = false
                };
            }

            if (user != null)
            {
                _mapper.Map(request, user);
                user.EmailConfirmed = false;
                user.IsDeleted = false;

                await _userManager.UpdateAsync(user);

            }
            else
            {
                user = _mapper.Map<CreateUserRequest, User>(request);
                user.UserName = phoneNumber;
                user.AvatarUrl = ImageConstant.Avatar;
                user.IsDeleted = false;
                user.EmailConfirmed = false;

                //var roleResult = await _userManager.AddToRoleAsync(user, RoleConstant.Customer);
                //if (!roleResult.Succeeded)
                //{
                //    return new ApiResult<bool>
                //    {
                //        Status = false,
                //        Message = "Lỗi không xác định khi gán vai trò",
                //        Data = false
                //    };
                //}
            }

            var otpCode = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user?.PhoneNumber);
            string subject = "HRM-HONIVY";
            string body = "Your verification code for HRM-HONIVY is " + otpCode;
            var send = await _authService.SendOtpMail(user.Email, subject, body);

            if (send == false)
            {
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Gửi thất bại",
                    Data = false,
                };
            }
            return new ApiResult<bool>
            {
                Status = true,
                Message = "Mã  đang được gửi đến mail của bạn",
                Data = true
            };

        }

        /// <summary>
        /// HCTH đăng ký tài khoản và nhân viên kích hoạt qua email để sử dụng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpPost("register-activate-by-email")]
        public async Task<ApiResult<bool>> RegisterActivateByEmail([FromBody] CreateUserRequest request)
        {
            var phoneNumber = request?.PhoneNumber?.Trim();
            var email = request?.Email?.Trim();

            var user = await _userManager.Users
                          .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber || u.Email == email);

            if (user != null && user.EmailConfirmed)
            {
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = user.PhoneNumber == phoneNumber ?
                              "Số điện thoại đã tồn tại trong hệ thống" : "Email đã tồn tại trong hệ thống",
                    Data = false
                };
            }

            if (user != null)
            {
                _mapper.Map(request, user);
                user.EmailConfirmed = false;
                user.IsDeleted = false;
                user.UserName = user.Email;
                user.PasswordHash = null;
                user.ActivationCode = Guid.NewGuid().ToString();
                user.ActivationExpiry = DateTime.Now.AddHours(24);

                await _userManager.UpdateAsync(user);

            }
            else
            {
                user = _mapper.Map<CreateUserRequest, User>(request);
                user.UserName = phoneNumber;
                user.AvatarUrl = ImageConstant.Avatar;
                user.UserName = user.Email;
                user.IsDeleted = false;
                user.EmailConfirmed = false;
                user.PasswordHash = null;
                user.ActivationCode = Guid.NewGuid().ToString();
                user.ActivationExpiry = DateTime.Now.AddHours(24);
                user.EmployeeId = request.EmployeeId;


                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return new ApiResult<bool>
                    {
                        Status = false,
                        Message = "Lỗi không xác định khi tạo người dùng",
                        Data = false
                    };
                }

                //var roleResult = await _userManager.AddToRoleAsync(user, RoleConstant.Customer);
                //if (!roleResult.Succeeded)
                //{
                //    return new ApiResult<bool>
                //    {
                //        Status = false,
                //        Message = "Lỗi không xác định khi gán vai trò",
                //        Data = false
                //    };
                //}
            }

            // Tạo link kích hoạt
            var activationLink = BuildActivationLink(request.UrlClient, user.Email, user.ActivationCode);

            string subject = BuildActivationMailSubject();

            string body = BuildActivationMailBody(user, activationLink);

            Task.Run(() => _authService.SendOtpMail(user.Email, subject, body));

            return new ApiResult<bool>
            {
                Status = true,
                Message = $"Tài khoản đã được tạo và email kích hoạt đã được gửi",
                Data = true,
            };

        }

        /// <summary>
        /// HCTH đăng ký nhiều tài khoản và nhân viên kích hoạt qua email để sử dụng
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        [HttpPost("register-activate-by-email-multiple")]
        public async Task<ApiResult<bool>> RegisterActivateByEmailMultiple([FromBody] List<CreateUserRequest> requests)
        {
            var results = new List<ApiResult<bool>>();

            foreach (var request in requests)
            {
                var phoneNumber = request?.PhoneNumber?.Trim();
                var email = request?.Email?.Trim();

                var user = await _userManager.Users
                                  .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber || u.Email == email);

                if (user != null && user.EmailConfirmed)
                {
                    results.Add(new ApiResult<bool>
                    {
                        Status = false,
                        Message = user.PhoneNumber == phoneNumber ?
                                  "Số điện thoại đã tồn tại trong hệ thống" : "Email đã tồn tại trong hệ thống",
                        Data = false
                    });
                    continue;
                }

                if (user != null)
                {
                    _mapper.Map(request, user);
                    user.EmailConfirmed = false;
                    user.IsDeleted = false;
                    user.UserName = user.Email;
                    user.PasswordHash = null;
                    user.ActivationCode = Guid.NewGuid().ToString();
                    user.ActivationExpiry = DateTime.Now.AddHours(24);

                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    user = _mapper.Map<CreateUserRequest, User>(request);
                    user.UserName = phoneNumber;
                    user.AvatarUrl = ImageConstant.Avatar;
                    user.UserName = user.Email;
                    user.IsDeleted = false;
                    user.EmailConfirmed = false;
                    user.PasswordHash = null;
                    user.ActivationCode = Guid.NewGuid().ToString();
                    user.ActivationExpiry = DateTime.Now.AddHours(24);
                    user.EmployeeId = request.EmployeeId;

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        results.Add(new ApiResult<bool>
                        {
                            Status = false,
                            Message = "Lỗi không xác định khi tạo người dùng",
                            Data = false
                        });
                        continue;
                    }

                    //var roleResult = await _userManager.AddToRoleAsync(user, RoleConstant.Customer);
                    //if (!roleResult.Succeeded)
                    //{
                    //    results.Add(new ApiResult<bool>
                    //    {
                    //        Status = false,
                    //        Message = "Lỗi không xác định khi gán vai trò",
                    //        Data = false
                    //    });
                    //    continue;
                    //}
                }

                // Tạo link kích hoạt
                var activationLink = BuildActivationLink(request.UrlClient, user.Email, user.ActivationCode);

                string subject = BuildActivationMailSubject();

                string body = BuildActivationMailBody(user, activationLink);

                Task.Run(() => _authService.SendOtpMail(user.Email, subject, body));

                results.Add(new ApiResult<bool>
                {
                    Status = true,
                    Message = $"Tài khoản {user.Email} đã được tạo và email kích hoạt đã được gửi",
                    Data = true,
                });
            }

            return new ApiResult<bool>
            {
                Status = results.All(r => r.Status),
                Message = results.All(r => r.Status) ? "Tất cả tài khoản đã được tạo và email kích hoạt đã được gửi." : "Có lỗi xảy ra trong quá trình đăng ký.",
                Data = results.All(r => r.Status),
            };
        }


        /// <summary>
        /// Là nhân viên/người dùng tôi muốn thiết lập mật khẩu đăng nhập tài khoản và kích hoạt tài khoản
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpPost("set-password")]
        public async Task<ApiResult<bool>> SetPassword([FromBody] SetPasswordRequest request)
        {
            // Kiểm tra xem email có tồn tại trong hệ thống không
            var user = await _userManager.Users
                            .Include(u => u.Employee)
                            .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Tài khoản không tồn tại trong hệ thống",
                    Data = false
                };
            }

            // Kiểm tra mã kích hoạt có hợp lệ không
            if (user.ActivationCode != request.ActivationCode)
            {
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Mã kích hoạt không hợp lệ",
                    Data = false
                };
            }

            // Kiểm tra xem mã kích hoạt có hết hạn không
            if (user.ActivationExpiry < DateTime.UtcNow)
            {
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Mã kích hoạt đã hết hạn",
                    Data = false
                };
            }

            // Kiểm tra xem tài khoản đã kích hoạt chưa
            if (user.EmailConfirmed)
            {
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Tài khoản đã được kích hoạt rồi không cần kích hoạt nữa",
                    Data = false
                };
            }

            // Đặt mật khẩu mới cho người dùng
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.NewPassword);
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Đặt mật khẩu không thành công",
                    Data = false
                };
            }

            // Cập nhật trạng thái email đã xác nhận và kích hoạt tài khoản
            user.EmailConfirmed = true;
            user.IsActivated = true;

            if (user.Employee != null)
            {
                user.Employee.AccountStatus = AccountStatus.Active;
            }
            await _userManager.UpdateAsync(user);

            return new ApiResult<bool>
            {
                Status = true,
                Message = "Mật khẩu đã được thiết lập thành công",
                Data = true
            };
        }

        /// <summary>
        /// Là admin/HR tôi muốn gửi lại email kích hoạt
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>


        [HttpPost("resend-activation-email")]
        public async Task<ApiResult<List<string>>> ResendActivationEmail([FromBody] ResendActivationRequest request)
        {
            var sentEmails = new List<string>();
            var notFoundEmails = new List<string>();
            var alreadyActivatedEmails = new List<string>();

            foreach (var email in request.Emails)
            {
                // Kiểm tra người dùng có tồn tại trong hệ thống không
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    notFoundEmails.Add(email);
                    continue;
                }

                // Kiểm tra xem người dùng đã kích hoạt email chưa
                if (user.EmailConfirmed)
                {
                    alreadyActivatedEmails.Add(email);
                    continue;
                }

                // Tạo lại mã kích hoạt và thời gian hết hạn
                user.ActivationCode = Guid.NewGuid().ToString();
                user.ActivationExpiry = DateTime.Now.AddHours(24);
                user.UserName = email;

                // Cập nhật lại người dùng
                await _userManager.UpdateAsync(user);

                // Tạo link kích hoạt
                var activationLink = BuildActivationLink(request.UrlClient, user.Email, user.ActivationCode);

                // Gửi email kích hoạt lại cho người dùng
                string subject = BuildActivationMailSubject();
                string body = BuildActivationMailBody(user, activationLink);

                Task.Run(() => _authService.SendOtpMail(user.Email, subject, body));

                sentEmails.Add(email);
            }

            var message = new StringBuilder();
            if (sentEmails.Any())
            {
                message.Append($"Email kích hoạt đã được gửi đến: {string.Join(", ", sentEmails)}.");
            }
            if (notFoundEmails.Any())
            {
                message.Append($" Không tìm thấy người dùng với email: {string.Join(", ", notFoundEmails)}.");
            }
            if (alreadyActivatedEmails.Any())
            {
                message.Append($" Tài khoản đã kích hoạt: {string.Join(", ", alreadyActivatedEmails)}.");
            }

            return new ApiResult<List<string>>
            {
                Status = sentEmails.Any(),
                Message = message.ToString(),
                Data = sentEmails
            };
        }



        [HttpPost("confirm-email-otp-register")]
        public async Task<ApiResult<bool>> ConfirmEmailOtpRegister([FromBody] VerifyMailOtpRegisterRequest request)
        {
            var user = await _userManager.Users
                       .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Không tìm thấy người dùng",
                    Data = false
                };
            }

            var otpValid = await _userManager.VerifyChangePhoneNumberTokenAsync(user, request.Otp, user.PhoneNumber);

            if (!otpValid)
            {

                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "OTP không chính xác",
                    Data = false
                };
            }

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return new ApiResult<bool>
            {
                Status = true,
                Message = "Xác nhận OTP thành công",
                Data = true
            };
        }

        /// <summary>
        /// Gửi OTP để xác thực cho Là nhân viên/người dùng tôi muốn lấy lại mật khẩu khi quên
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpPost("send-email-otp")]
        public async Task<ApiResult<bool>> SendMailOtp([FromBody] SendMailRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.ToEmail.Trim());

            if (user == null)
            {
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Tài khoản không tồn tại trong hệ thống",
                    Data = false
                };

            }

            var otpCode = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user?.PhoneNumber);


            string subject = ActivationCompanyName;
            string body = "Mã OTP của bạn là: " + otpCode;

            //var FormatPhoneNumber = "+84" + user.PhoneNumber.Substring(1);

            //var send = await _authService.SendOtpMail(user.Email, subject, body);

            Task.Run(() => _authService.SendOtpMail(user.Email, subject, body));


            return new ApiResult<bool>
            {
                Status = true,
                Message = "Mã đang được gửi đến mail của bạn",
                Data = true
            };
        }

        [HttpPost("refresh-token")]
        public async Task<ApiResult<LoginResult>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var loginResult = await _authService.RefreshToken(request.RefreshToken);

                return new ApiResult<LoginResult>()
                {
                    Status = true,
                    Message = "Làm mới thành công!",
                    Data = loginResult
                };
            }
            catch (BadHttpRequestException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<ApiResult<bool>> Logout()
        {
            try
            {
                var isLogout = await _authService.Logout();

                return new ApiResult<bool>()
                {
                    Status = true,
                    Message = "Đăng xuất thành công!",
                    Data = isLogout
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
            catch (BadHttpRequestException ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Xác thực OTP
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("verify-otp")]
        public async Task<ApiResult<bool>> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            // Kiểm tra email hợp lệ
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Tài khoản không tồn tại trong hệ thống",
                    Data = false
                };
            }

            // Xác thực OTP
            var success = await _userService.VerifyEmailWithOtp(request.Email, request.Otp);
            if (!success.Success)
            {
                return new ApiResult<bool>
                {
                    Status = false,
                    Message = "Mã OTP không hợp lệ hoặc đã hết hạn",
                    Data = false
                };
            }

            return new ApiResult<bool>
            {
                Status = true,
                Message = "OTP hợp lệ",
                Data = true
            };
        }

    }
}
