using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Domain.Data.Seeder
{
    public interface IDbSeeder
    {
        Task SeedAsync();
    }
}
