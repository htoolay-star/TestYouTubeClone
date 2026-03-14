using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeClone.Shared.Constants;
using YouTubeClone.Domain.Data;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Shared.DTOs;
using static System.Net.WebRequestMethods;

namespace YouTubeClone.Domain.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly string _connectionString;

        public UserRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new Exception(AuthMessages.Infrastructure.ConnectionStringNotFound);
        }

        #region EF Core Methods (Read & Basic Add)

        public async Task<User?> GetByPublicIdAsync(Guid publicId)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.PublicId == publicId && !u.IsDeleted);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task AddAsync(User user) => await _context.Users.AddAsync(user);

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        #endregion

        #region Dapper Methods (Write & Complex Queries)

        public async Task<Guid?> CreateUserWithSpAsync(User user, byte defaultRoleId)
        {
            using var connection = await GetOpenConnectionAsync();

            var parameters = new
            {
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Bio = user.Bio,
                DOB = user.DateOfBirth?.ToDateTime(TimeOnly.MinValue),
                Gender = user.Gender,
                DefaultRoleId = defaultRoleId,
            };

            var result = await connection.QuerySingleOrDefaultAsync<dynamic>(
                "[Identity].[usp_CreateUser]",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result != null ? (Guid)result.PublicId : null;
        }

        public async Task<bool> PatchUserAsync(Guid publicId, UpdateUserRequest request)
        {
            using var connection = await GetOpenConnectionAsync();
            var updateFields = new List<string>();
            var parameters = new DynamicParameters();

            parameters.Add("PublicId", publicId);

            if (request.Username != null) { updateFields.Add("Username = @Username"); parameters.Add("Username", request.Username); }
            if (request.Bio != null) { updateFields.Add("Bio = @Bio"); parameters.Add("Bio", request.Bio); }
            if (request.ProfilePictureUrl != null) { updateFields.Add("ProfilePictureUrl = @Pic"); parameters.Add("Pic", request.ProfilePictureUrl); }

            if (request.DateOfBirth != null)
            {
                updateFields.Add("DateOfBirth = @DOB");
                parameters.Add("DOB", request.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue));
            }

            if (request.Gender != null) { updateFields.Add("Gender = @Gender"); parameters.Add("Gender", request.Gender); }

            if (!updateFields.Any()) return false;

            var sql = $"UPDATE [Identity].[Users] SET {string.Join(", ", updateFields)}";

            return await connection.ExecuteAsync(sql, parameters) > 0;
        }

        public async Task<bool> UpdatePasswordHashAsync(Guid publicId, string newHash)
        {
            using var connection = await GetOpenConnectionAsync();
            var sql = @"
                UPDATE [Identity].[Users] 
                SET PasswordHash = @Hash
                WHERE PublicId = @PublicId";

            return await connection.ExecuteAsync(sql, new { Hash = newHash, PublicId = publicId }) > 0;
        }

        public async Task<bool> UpdateEmailAsync(Guid publicId, string newEmail)
        {
            using var connection = await GetOpenConnectionAsync();
            var sql = "UPDATE [Identity].[Users] SET Email = @Email WHERE PublicId = @PublicId";
            return await connection.ExecuteAsync(sql, new { Email = newEmail, PublicId = publicId }) > 0;
        }

        public async Task<bool> ExistsAsync(string email)
        {
            using var connection = await GetOpenConnectionAsync();
            var sql = "SELECT COUNT(1) FROM [Identity].[Users] WHERE Email = @Email";
            return await connection.ExecuteScalarAsync<int>(sql, new { Email = email }) > 0;
        }

        public async Task<bool> UpdateProfilePictureAsync(Guid publicId, string? imageUrl)
        {
            using var connection = await GetOpenConnectionAsync();
            const string sql = "UPDATE [Identity].[Users] SET ProfilePictureUrl = @ImageUrl, UpdatedAt = GETUTCDATE() WHERE PublicId = @PublicId";
            return await connection.ExecuteAsync(sql, new { ImageUrl = imageUrl, PublicId = publicId }) > 0;
        }

        public async Task<bool> SoftDeleteUserAsync(Guid publicId)
        {
            using var connection = await GetOpenConnectionAsync();
            const string sql = @"
                UPDATE [Identity].[Users] 
                SET IsDeleted = 1, 
                DeletedAt = GETUTCDATE(),
                RefreshToken = NULL,       
                RefreshTokenExpiryTime = NULL
                WHERE PublicId = @PublicId AND IsDeleted = 0";

            return await connection.ExecuteAsync(sql, new { PublicId = publicId }) > 0;
        }

        public async Task<bool> HardDeleteUserAsync(Guid publicId)
        {
            using var connection = await GetOpenConnectionAsync();
            const string sql = "DELETE FROM [Identity].[Users] WHERE PublicId = @PublicId";
            return await connection.ExecuteAsync(sql, new { PublicId = publicId }) > 0;
        }

        public async Task<bool> VerifyEmailAsync(Guid publicId)
        {
            using var connection = await GetOpenConnectionAsync();
            const string sql = @"
                UPDATE [Identity].[Users] 
                SET IsEmailVerified = 1
                WHERE PublicId = @PublicId";

            return await connection.ExecuteAsync(sql, new { PublicId = publicId }) > 0;
        }

        private async Task<IDbConnection> GetOpenConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        #endregion
    }
}