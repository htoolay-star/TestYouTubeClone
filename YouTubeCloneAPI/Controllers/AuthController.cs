using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Shared.DTOs;
using YouTubeClone.Shared.Constants;

namespace YouTubeClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        #region Authentication Flow

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var publicId = await authService.RegisterAsync(request);
            return Ok(new
            {
                message = AuthMessages.Success.Register,
                publicId
            });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            await authService.VerifyOtpAsync(request.PublicId, request.Otp);
            return Ok(new { message = AuthMessages.Success.OtpVerify });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await authService.LoginAsync(request);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
        {
            var response = await authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);
            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var publicId = GetUserPublicId();
            await authService.LogoutAsync(publicId);
            return Ok(new { message = AuthMessages.Success.Logout });
        }

        #endregion

        #region Profile Management

        [Authorize]
        [HttpPatch("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request)
        {
            await authService.UpdateProfileAsync(GetUserPublicId(), request);
            return Ok(new { message = AuthMessages.Success.ProfileUpdate });
        }

        [Authorize]
        [HttpPost("upload-profile-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            var imageUrl = await authService.UploadProfilePictureAsync(GetUserPublicId(), file);
            return Ok(new { url = imageUrl, message = AuthMessages.Success.ProfilePicture });
        }

        [Authorize]
        [HttpDelete("remove-profile-picture")]
        public async Task<IActionResult> RemoveProfilePicture()
        {
            await authService.RemoveProfilePictureAsync(GetUserPublicId());
            return Ok(new { message = AuthMessages.Success.ProfilePictureRemoved });
        }

        #endregion

        #region Password & Email Recovery

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            await authService.ChangePasswordAsync(GetUserPublicId(), request);
            return Ok(new { message = AuthMessages.Success.PasswordUpdate });
        }

        [Authorize]
        [HttpPost("request-email-change")]
        public async Task<IActionResult> RequestEmailChange([FromBody] ChangeEmailRequest request)
        {
            await authService.RequestEmailChangeAsync(GetUserPublicId(), request);
            return Ok(new { message = AuthMessages.Success.EmailChangeRequestSent });
        }

        [Authorize]
        [HttpPost("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeRequest request)
        {
            await authService.ConfirmEmailChangeAsync(GetUserPublicId(), request.Otp);
            return Ok(new { message = AuthMessages.Success.EmailUpdate });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            await authService.ForgotPasswordAsync(request);
            return Ok(new { message = AuthMessages.Success.ForgotPassword });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await authService.ResetPasswordAsync(request);
            return Ok(new { message = AuthMessages.Success.PasswordReset });
        }

        #endregion

        #region Account Security

        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromQuery] bool permanent = false)
        {
            var publicId = GetUserPublicId();
            if (permanent)
            {
                await authService.PermanentDeleteAccountAsync(publicId);
                return Ok(new { message = AuthMessages.Success.AccountDeleted });
            }

            await authService.DeactivateAccountAsync(publicId);
            return Ok(new { message = AuthMessages.Success.AccountDeactivated });
        }

        [Authorize]
        [HttpGet("test-auth")]
        public IActionResult TestAuth() => Ok(new { message = AuthMessages.Success.Authorized });

        #endregion

        #region Private Helpers

        private Guid GetUserPublicId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                throw new Exception(AuthMessages.Security.UnauthorizedUserId);

            return Guid.Parse(userIdStr);
        }

        #endregion
    }
}