using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Shared.DTOs;
using YouTubeClone.Shared.Models;

namespace YouTubeClone.Domain.Services
{
    public class AuthService(IUnitOfWork uow, JwtSettings jwtSettings) : IAuthService
    {
        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            // ၁။ User ရှိမရှိ စစ်ဆေးသည်
            // Note: Repository ထဲမှာ GetByEmailAsync(email) logic လိုအပ်ပါလိမ့်မယ်
            var user = await uow.Users.GetByEmailAsync(request.Email);
            if (user == null) return null;

            // ၂။ Password မှန်မမှန် BCrypt ဖြင့် စစ်ဆေးသည်
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            // ၃။ JWT Token ထုတ်ပေးသည်
            return GenerateJwtToken(user);
        }

        private AuthResponse GenerateJwtToken(Entities.User user)
        {
            // ၁။ Role နာမည်ကို ယူခြင်း (User မှာ Role တစ်ခုထက်မက ရှိနိုင်လို့ ပထမဆုံးတစ်ခုကို ယူပါမယ်)
            var userRoleName = user.UserRoles.FirstOrDefault()?.Role?.RoleName ?? "User";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.PublicId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, userRoleName) // Role Claim ထည့်ခြင်း
            };

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

            // ၂။ AuthResponse ထဲကို Role ပါ ထည့်ပေးလိုက်ပါ
            return new AuthResponse(token, user.Username, user.Email, userRoleName, expiry);
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // ၁။ Email ရှိပြီးသားလား အရင်စစ်ဆေးမယ်
                if (await uow.Users.ExistsAsync(request.Email))
                {
                    // Email ရှိနေရင် logic အနေနဲ့ false ပြန်မယ်
                    return false;
                }

                // ၂။ Password ကို Hash လုပ်မယ်
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // ၃။ Entity တည်ဆောက်မယ်
                var newUser = new Entities.User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = hashedPassword,
                    Bio = request.Bio,
                    ProfilePictureUrl = request.ProfilePictureUrl,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    PublicId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // ၄။ Repository မှတစ်ဆင့် SP ကို ခေါ်မယ်
                // ဒီနေရာမှာ SQL Error တက်ခဲ့ရင် catch block ဆီကို တန်းရောက်သွားမှာပါ
                var rowsAffected = await uow.Users.CreateUserWithSpAsync(newUser, 1);

                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                // SQL နဲ့ ပတ်သက်တဲ့ Error တွေကို သီးသန့်ဖမ်းမယ်
                // ဥပမာ - Log ထုတ်တာမျိုး လုပ်နိုင်ပါတယ်
                throw new Exception("Database error occurred while registering user.", ex);
            }
            catch (Exception ex)
            {
                // တခြား အထွေထွေ Error များကို ဖမ်းမယ်
                throw new Exception("An unexpected error occurred during registration.", ex);
            }
        }
    }
}
