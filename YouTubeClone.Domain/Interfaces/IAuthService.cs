using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Shared.DTOs;

namespace YouTubeClone.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<bool> LogoutAsync(Guid publicId);
        Task<AuthResponse?> RefreshTokenAsync(string oldAccessToken, string refreshToken);
        Task<Guid?> RegisterAsync(RegisterRequest request);
        Task<bool> UpdateProfileAsync(Guid publicId, UpdateUserRequest request);
        Task<bool> ChangePasswordAsync(Guid publicId, ChangePasswordRequest request);
        Task RequestEmailChangeAsync(Guid publicId, ChangeEmailRequest request);
        Task<bool> ConfirmEmailChangeAsync(Guid publicId, string otp);
        Task<string> UploadProfilePictureAsync(Guid publicId, IFormFile file);
        Task<bool> RemoveProfilePictureAsync(Guid publicId);
        Task<bool> DeactivateAccountAsync(Guid publicId);
        Task<bool> PermanentDeleteAccountAsync(Guid publicId);
        Task<bool> VerifyOtpAsync(Guid publicId, string inputOtp);
        Task ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
