using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Shared.DTOs;

namespace YouTubeClone.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByPublicIdAsync(Guid publicId);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task AddAsync(User user);
        Task<bool> ExistsAsync(string email);
        Task<User?> GetByEmailAsync(string email);
        Task<Guid?> CreateUserWithSpAsync(User user, byte defaultRoleId);
        Task<bool> PatchUserAsync(Guid publicId, UpdateUserRequest request);
        Task<bool> UpdatePasswordHashAsync(Guid publicId, string newHash);
        Task<bool> UpdateEmailAsync(Guid publicId, string newEmail);
        Task<bool> UpdateProfilePictureAsync(Guid publicId, string? imageUrl);
        Task<bool> SoftDeleteUserAsync(Guid publicId);
        Task<bool> HardDeleteUserAsync(Guid publicId);
        Task<bool> VerifyEmailAsync(Guid publicId);
    }
}
