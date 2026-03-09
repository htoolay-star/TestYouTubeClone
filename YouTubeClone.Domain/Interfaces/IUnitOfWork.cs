using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        Task<int> CompleteAsync(); // SaveChangesAsync() အစား သုံးမည်
    }
}
