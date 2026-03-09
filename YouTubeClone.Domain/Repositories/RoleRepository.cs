using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using YouTubeClone.Domain.Data;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Domain.Repositories
{
    public class RoleRepository(AppDbContext context) : IRoleRepository
    {
        public async Task<Role?> GetByNameAsync(string name)
            => await context.Roles.FirstOrDefaultAsync(r => r.RoleName == name);

        public async Task AddAsync(Role role) => await context.Roles.AddAsync(role);

        public async Task<bool> AnyAsync(string name)
            => await context.Roles.AnyAsync(r => r.RoleName == name);
    }
}
