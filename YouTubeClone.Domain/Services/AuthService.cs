using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Shared.Constants;
using YouTubeClone.Shared.DTOs;
using YouTubeClone.Shared.DTOs.Cache;
using YouTubeClone.Shared.Enums;
using YouTubeClone.Shared.Extensions;
using YouTubeClone.Shared.Models;

namespace YouTubeClone.Domain.Services
{
    public class AuthService(
            IUnitOfWork uow, 
            JwtSettings jwtSettings, 
            IMapper mapper, 
            IFileService fileService, 
            IEmailService emailService,
            IDistributedCache cache) : IAuthService
    {
        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await uow.Users.GetByEmailAsync(request.Email);

            if (user == null) throw new Exception(AuthMessages.Auth.UserNotFound);

            if (user.AccountStatus == (byte)AccountStatus.Suspended)
            {
                throw new Exception(AuthMessages.Auth.AccountSuspended);
            }

            if (user.AccountStatus == (byte)AccountStatus.Banned)
            {
                throw new Exception(AuthMessages.Auth.AccountBanned);
            }

            if (user.AccountStatus == (byte)AccountStatus.Pending)
            {
                throw new Exception(AuthMessages.Auth.AccountPending);
            }

            if (user.IsDeleted)
            {
                throw new Exception(AuthMessages.Auth.AccountDeactivated);
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                var remainingMinutes = (int)Math.Ceiling((user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes);

                throw new Exception(string.Format(AuthMessages.Auth.AccountLocked, remainingMinutes));
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                user.AccessFailedCount++;
                
                if (user.AccessFailedCount >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                    user.AccessFailedCount = 0;
                }

                await uow.CompleteAsync();

                throw new Exception(AuthMessages.Auth.InvalidCredentials);
            }

            user.AccessFailedCount = 0;
            user.LockoutEnd = null;

            if (!user.IsEmailVerified)
            {
                throw new Exception(AuthMessages.Auth.EmailNotVerified);
            }

            user.IsOnline = true;
            user.LastLoginAt = DateTime.UtcNow;

            var userCache = mapper.Map<UserCacheItem>(user);

            var accessTokenResult = GenerateJwtToken(userCache);

            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(AuthConstants.Security.RefreshTokenBytes));

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenDurationInDays);

            await uow.CompleteAsync();

            var cacheKey = string.Format(AuthConstants.CacheKeys.RefreshToken, refreshToken);
            await cache.SetObjectAsync(cacheKey, userCache, TimeSpan.FromDays(jwtSettings.RefreshTokenDurationInDays));

            return new AuthResponse(
                accessTokenResult.Token,
                refreshToken,
                user.Username,
                user.Email,
                accessTokenResult.Role,
                accessTokenResult.Expiration);
        }

        public async Task<bool> LogoutAsync(Guid publicId)
        {
            var user = await uow.Users.GetByPublicIdAsync(publicId);
            if (user == null) throw new Exception(AuthMessages.Auth.UserNotFound);

            user.IsOnline = false;

            if (string.IsNullOrEmpty(user.RefreshToken))
            {
                var cacheKey = string.Format(AuthConstants.CacheKeys.RefreshToken, user.RefreshToken);
                await cache.RemoveAsync(cacheKey);

                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
            }

            var affectedRows = await uow.CompleteAsync();

            return true;
        }

        public async Task<AuthResponse?> RefreshTokenAsync(string oldAccessToken, string refreshToken)
        {
            var cacheKey = string.Format(AuthConstants.CacheKeys.RefreshToken, refreshToken);

            var cachedUser = await cache.GetObjectAsync<UserCacheItem>(cacheKey);
            Entities.User? userEntity = null;

            if (cachedUser == null)
            {
                userEntity = await uow.Users.GetByRefreshTokenAsync(refreshToken);

                if (userEntity == null || userEntity.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    throw new Exception(AuthMessages.Security.InvalidRefreshToken);
                }

                cachedUser = mapper.Map<UserCacheItem>(userEntity);
            }
            else
            {
                userEntity = await uow.Users.GetByRefreshTokenAsync(refreshToken);
            }

            if (userEntity == null) throw new Exception(AuthMessages.Security.InvalidRefreshToken);

            var newAccessToken = GenerateJwtToken(cachedUser);
            var newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(AuthConstants.Security.RefreshTokenBytes));

            await cache.RemoveAsync(cacheKey);

            userEntity.RefreshToken = newRefreshToken;
            userEntity.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenDurationInDays);
            await uow.CompleteAsync();

            cachedUser.RefreshToken = newRefreshToken;
            cachedUser.RefreshTokenExpiryTime = userEntity.RefreshTokenExpiryTime;

            var newCacheKey = string.Format(AuthConstants.CacheKeys.RefreshToken, newRefreshToken);
            await cache.SetObjectAsync(newCacheKey, cachedUser, TimeSpan.FromDays(jwtSettings.RefreshTokenDurationInDays));

            return new AuthResponse(
                newAccessToken.Token,
                newRefreshToken,
                cachedUser.Username,
                cachedUser.Email,
                newAccessToken.Role,
                newAccessToken.Expiration);
        }

        private AuthResponse GenerateJwtToken(UserCacheItem user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.PublicId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
            };

            foreach (var roleName in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddMinutes(jwtSettings.DurationInMinutes);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new AuthResponse(
                token,
                "",
                user.Username,
                user.Email,
                user.Roles.FirstOrDefault() ?? AuthRoles.User,
                tokenDescriptor.ValidTo);
        }

        public async Task<Guid?> RegisterAsync(RegisterRequest request)
        {
            if (await uow.Users.ExistsAsync(request.Email))
            {
                throw new Exception(AuthMessages.Auth.EmailAlreadyRegistered);
            }

            var newUser = mapper.Map<Entities.User>(request);
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            newUser.IsEmailVerified = false;

            var publicId = await uow.Users.CreateUserWithSpAsync(newUser, AuthConstants.Roles.UserRoleId);

            var otp = GenerateSecureOtp();

            var cacheKey = string.Format(AuthConstants.CacheKeys.Registration, publicId);
            var cacheData = new OtpCacheItem { Otp = otp, Email = request.Email };

            await cache.SetObjectAsync(cacheKey, cacheData, TimeSpan.FromMinutes(AuthConstants.CacheKeys.OtpExpirationMinutes));

            if (publicId == null) throw new Exception(AuthMessages.Auth.RegistrationFailed);

            var emailBody = string.Format(EmailTemplates.OtpEmailBody, otp);
            await emailService.SendEmailAsync(request.Email, AuthMessages.Verification.VerifyEmailSubject, emailBody);

            return publicId;
        }

        public async Task<bool> UpdateProfileAsync(Guid publicId, UpdateUserRequest request)
        {
            return await uow.Users.PatchUserAsync(publicId, request);
        }

        public async Task<bool> ChangePasswordAsync(Guid publicId, ChangePasswordRequest request)
        {
            var user = await uow.Users.GetByPublicIdAsync(publicId);
            if (user == null) throw new Exception(AuthMessages.Auth.UserNotFound);

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                throw new Exception(AuthMessages.Auth.InvalidCurrentPassword);
            }

            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            return await uow.Users.UpdatePasswordHashAsync(publicId, newHash);
        }

        public async Task RequestEmailChangeAsync(Guid publicId, ChangeEmailRequest request)
        {
            var user = await uow.Users.GetByPublicIdAsync(publicId);
            if (user == null) throw new Exception(AuthMessages.Auth.UserNotFound);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception(AuthMessages.Auth.InvalidPasswordConfirm);
            }

            if (await uow.Users.ExistsAsync(request.NewEmail))
            {
                throw new Exception(AuthMessages.Auth.EmailInUse);
            }

            var otp = GenerateSecureOtp();

            var cacheKey = string.Format(AuthConstants.CacheKeys.EmailChange, publicId);
            var cacheData = new OtpCacheItem { NewEmail = request.NewEmail, Otp = otp };

            await cache.SetObjectAsync(cacheKey, cacheData, TimeSpan.FromMinutes(AuthConstants.CacheKeys.OtpExpirationMinutes));

            await emailService.SendEmailAsync(request.NewEmail, AuthMessages.Verification.EmailChangeSubject, string.Format(EmailTemplates.EmailChangeBody, otp));
        }

        public async Task<bool> ConfirmEmailChangeAsync(Guid publicId, string otp)
        {
            var cacheKey = string.Format(AuthConstants.CacheKeys.EmailChange, publicId);

            var cachedData = await VerifyAndGetOtpCache(cacheKey, otp);

            var success = await uow.Users.UpdateEmailAsync(publicId, cachedData.NewEmail);

            if (success)
            {
                await cache.RemoveAsync(cacheKey);

                var user = await uow.Users.GetByPublicIdAsync(publicId);

                if (user != null && !string.IsNullOrEmpty(user.RefreshToken))
                {
                    var rtCacheKey = string.Format(AuthConstants.CacheKeys.RefreshToken, user.RefreshToken);
                    await cache.RemoveAsync(rtCacheKey);

                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = null;
                    await uow.CompleteAsync();
                }
            }

            return success;
        }

        public async Task<string> UploadProfilePictureAsync(Guid publicId, IFormFile file)
        {
            var user = await uow.Users.GetByPublicIdAsync(publicId);
            if (user == null) throw new Exception(AuthMessages.Auth.UserNotFound);

            string newImageUrl = await fileService.SaveFileAsync(file, AuthConstants.Files.AllowedImageExtensions);

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                fileService.DeleteFile(user.ProfilePictureUrl);
            }

            var success = await uow.Users.UpdateProfilePictureAsync(publicId, newImageUrl);
            if (!success) throw new Exception(AuthMessages.Auth.ProfileUpdateFailed);

            return newImageUrl;
        }

        public async Task<bool> RemoveProfilePictureAsync(Guid publicId)
        {
            var user = await uow.Users.GetByPublicIdAsync(publicId);
            if (user == null) throw new Exception(AuthMessages.Auth.UserNotFound);

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                fileService.DeleteFile(user.ProfilePictureUrl);
            }

            return await uow.Users.UpdateProfilePictureAsync(publicId, null);
        }

        public async Task<bool> DeactivateAccountAsync(Guid publicId)
        {
            var result = await uow.Users.SoftDeleteUserAsync(publicId);
            if (!result) throw new Exception(AuthMessages.Auth.UserNotFound);
            return result;
        }

        public async Task<bool> PermanentDeleteAccountAsync(Guid publicId)
        {
            var user = await uow.Users.GetByPublicIdAsync(publicId);
            if (user == null) throw new Exception(AuthMessages.Auth.UserNotFound);

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                fileService.DeleteFile(user.ProfilePictureUrl);
            }

            return await uow.Users.HardDeleteUserAsync(publicId);
        }

        public async Task<bool> VerifyOtpAsync(Guid publicId, string inputOtp)
        {
            var cacheKey = string.Format(AuthConstants.CacheKeys.Registration, publicId);

            var cachedData = await VerifyAndGetOtpCache(cacheKey, inputOtp);

            var success = await uow.Users.VerifyEmailAsync(publicId);

            if (success)
            {
                await cache.RemoveAsync(cacheKey);
            }

            return success;
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await uow.Users.GetByEmailAsync(request.Email);

            if (user == null)
                throw new Exception(AuthMessages.Verification.PasswordResetUserNotFound);

            var otp = GenerateSecureOtp();

            var cacheKey = string.Format(AuthConstants.CacheKeys.PasswordReset, user.PublicId);
            var cacheData = new OtpCacheItem { Otp = otp, Email = request.Email };

            await cache.SetObjectAsync(cacheKey, cacheData, TimeSpan.FromMinutes(AuthConstants.CacheKeys.OtpExpirationMinutes));

            var emailBody = string.Format(EmailTemplates.PasswordResetBody, otp);

            await emailService.SendEmailAsync(request.Email, AuthMessages.Verification.PasswordResetSubject, emailBody);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await uow.Users.GetByEmailAsync(request.Email);

            if (user == null) throw new Exception(AuthMessages.Auth.UserNotFound);

            var cacheKey = string.Format(AuthConstants.CacheKeys.PasswordReset, user.PublicId);

            await VerifyAndGetOtpCache(cacheKey, request.Otp);

            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            var success = await uow.Users.UpdatePasswordHashAsync(user.PublicId, newHash);

            if (success)
            {
                await cache.RemoveAsync(cacheKey);
            }

            return true;
        }

        private string GenerateSecureOtp()
        {
            var bytes = RandomNumberGenerator.GetBytes(4);
            uint randomValue = BitConverter.ToUInt32(bytes, 0);

            var otp = (randomValue % 1000000).ToString("D6");
            return otp;
        }

        private async Task<OtpCacheItem> VerifyAndGetOtpCache(string cacheKey, string inputOtp)
        {
            var cachedData = await cache.GetObjectAsync<OtpCacheItem>(cacheKey);

            if (cachedData == null)
            {
                throw new Exception(AuthMessages.Verification.OtpNotFound);
            }

            if (cachedData.Otp != inputOtp)
            {
                throw new Exception(AuthMessages.Verification.InvalidOtp);
            }

            return cachedData;
        }
    }
}
