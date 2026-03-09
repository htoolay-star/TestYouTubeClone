using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Domain.Entities;

namespace YouTubeClone.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
        Task AddAsync(Role role);
        Task<bool> AnyAsync(string name);
    }
}
