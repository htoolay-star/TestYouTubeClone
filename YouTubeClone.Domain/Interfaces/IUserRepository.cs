using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Domain.Entities;

namespace YouTubeClone.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<bool> ExistsAsync(string email);
        Task<User?> GetByEmailAsync(string email);
        Task<int> CreateUserWithSpAsync(User user, byte defaultRoleId);
    }
}
