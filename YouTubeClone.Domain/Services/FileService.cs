using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Shared.Constants;

namespace YouTubeClone.Domain.Services
{

    public class FileService(IWebHostEnvironment environment) : IFileService
    {
        public async Task<string> SaveFileAsync(IFormFile file, string[] allowedExtensions)
        {
            // ၁။ Validation (Empty & Size)
            if (file == null || file.Length == 0)
                throw new Exception(AuthMessages.Files.FileEmpty);

            if (file.Length > AuthConstants.Files.MaxImageSize)
                throw new Exception(AuthMessages.Files.FileSizeExceeded);

            // ၂။ Extension Check
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                throw new Exception(string.Format(AuthMessages.Files.InvalidFileType, string.Join(", ", allowedExtensions)));
            }

            // ၃။ Folder Setup
            // wwwroot ထဲက uploads/profiles ကို ညွှန်းမယ်
            string folderPath = Path.Combine(environment.WebRootPath, AuthConstants.Files.ProfileUploadFolder);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // ၄။ Unique Name & Path
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string fullPath = Path.Combine(folderPath, uniqueFileName);

            // ၅။ Save
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // DB မှာ သိမ်းဖို့ Relative Path ကို ပြန်ပေးမယ် (ဥပမာ - /uploads/profiles/abc.jpg)
            return $"/{AuthConstants.Files.ProfileUploadFolder}/{uniqueFileName}";
        }

        public void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;

            // Path ထဲက '/' တွေကို ရှင်းပြီး Absolute Path ပြောင်းမယ်
            var absolutePath = Path.Combine(environment.WebRootPath, relativePath.TrimStart('/'));

            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }
        }
    }
}
