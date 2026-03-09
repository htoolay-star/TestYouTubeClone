using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using YouTubeClone.Domain.Data;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Domain.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task AddAsync(User user) => await context.Users.AddAsync(user);
        public async Task<bool> ExistsAsync(string email)
            => await context.Users.AnyAsync(u => u.Email == email);

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<int> CreateUserWithSpAsync(User user, byte defaultRoleId)
        {
            // EF Connection ကို Dapper အတွက် ယူသုံးခြင်း
            var connection = context.Database.GetDbConnection();

            var parameters = new
            {
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                DOB = user.DateOfBirth?.ToDateTime(TimeOnly.MinValue),
                Gender = user.Gender,
                DefaultRoleId = defaultRoleId
            };

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            // Dapper Syntax က ပိုတိုပြီး Parameter mapping လုပ်ရတာ လွယ်ကူတယ်
            var result = await connection.QuerySingleAsync<dynamic>(
                "[Identity].[usp_CreateUser]",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.NewUserId > 0 ? 1 : 0;
        }
    }
}
