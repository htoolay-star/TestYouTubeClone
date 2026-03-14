using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Domain.Data;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Domain.Repositories
{
    public class UnitOfWork(AppDbContext context, IConfiguration configuration) : IUnitOfWork
    {
        // Lazy Initialization အတွက် ပိုရှင်းလင်းသော ပုံစံ
        public IUserRepository Users => _users ??= new UserRepository(context, configuration);
        public IRoleRepository Roles => _roles ??= new RoleRepository(context);

        private IUserRepository? _users;
        private IRoleRepository? _roles;

        public async Task<int> CompleteAsync() => await context.SaveChangesAsync();

        public void Dispose()
        {
            context.Dispose();
            // Memory leak မဖြစ်အောင် suppress လုပ်တာ မှန်ပါတယ်
            GC.SuppressFinalize(this);
        }
    }
}
