using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Domain.Interfaces
{
    public interface IFileService
    {
        // ပုံကို သိမ်းပြီး ရလာတဲ့ URL/Path ကို string အနေနဲ့ ပြန်ပေးမယ်
        Task<string> SaveFileAsync(IFormFile file, string[] allowedExtensions);
        void DeleteFile(string filePath);
    }
}
