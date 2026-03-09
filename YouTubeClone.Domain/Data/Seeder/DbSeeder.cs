using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Shared.Models;

namespace YouTubeClone.Domain.Data.Seeder
{
    public class DbSeeder(IUnitOfWork uow, SeedSettings settings) : IDbSeeder
    {
        public async Task SeedAsync()
        {
            // အဆင့် (၁) - Roles များကို Seed လုပ်ခြင်း
            await SeedRolesAsync();

            // အဆင့် (၂) - Admin Account ကို Seed လုပ်ခြင်း
            await SeedAdminAsync();

            // အဆင့် (၃) - အပြောင်းအလဲအားလုံးကို တစ်ခါတည်း Save လုပ်ခြင်း
            await uow.CompleteAsync();
        }

        private async Task SeedRolesAsync()
        {
            string[] roleNames = ["Admin", "User", "Creator"];

            foreach (var roleName in roleNames)
            {
                // Unit of Work မှတစ်ဆင့် Roles Repository ကို ခေါ်သုံးခြင်း
                if (!await uow.Roles.AnyAsync(roleName))
                {
                    await uow.Roles.AddAsync(new Role { RoleName = roleName });
                }
            }
        }

        private async Task SeedAdminAsync()
        {
            // Admin email ရှိမရှိ အရင်စစ်သည်
            if (await uow.Users.ExistsAsync(settings.AdminEmail)) return;

            // BCrypt သုံးပြီး Password ကို Hash လုပ်သည်
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(settings.AdminPassword);

            // Admin Role ကို ရှာသည်
            var adminRole = await uow.Roles.GetByNameAsync("Admin");

            if (adminRole == null) return;

            var adminUser = new User
            {
                PublicId = Guid.NewGuid(),
                Username = settings.AdminUserName,
                Email = settings.AdminEmail,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            // UserRole object ကို တိုက်ရိုက်ဆောက်ပြီး User ရဲ့ UserRoles collection ထဲ ထည့်ခြင်း
            adminUser.UserRoles.Add(new UserRole 
            { 
                Role = adminRole,
                User = adminUser,
                AssignedAt = DateTime.UtcNow
                // AssignedByUserId က null ထားခဲ့လို့ရပါတယ် (System ကနေ seed တာမို့လို့)
            });

            await uow.Users.AddAsync(adminUser);
        }
    }
}
