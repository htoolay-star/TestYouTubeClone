using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.Constants
{
    public static class AuthConstants
    {
        // 1. Roles & Permissions
        public static class Roles
        {
            // Role IDs (Database IDs)
            public const int UserRoleId = 1;
            public const int CreatorRoleId = 2;
            public const int AdminRoleId = 3;

            // Role Names (For Authorization Policies)
            public const string User = "User";
            public const string Creator = "Creator";
            public const string Admin = "Admin";
        }

        // 2. Token & Security Settings
        public static class Security
        {
            public const int OtpExpiryMinutes = 15;
            public const int RefreshTokenBytes = 64;
            public const int AccessTokenExpiryMinutes = 60; // လိုရင်သုံးဖို့
            public const int RefreshTokenExpiryDays = 7;
        }

        // 3. File & Upload Settings
        public static class Files
        {
            public static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
            public const string ProfileUploadFolder = "uploads/profiles";
            public const string VideoUploadFolder = "uploads/videos"; // အနာဂတ်အတွက်
            public const long MaxImageSize = 2 * 1024 * 1024; // 2MB
        }

        public static class CacheKeys
        {
            public const string Registration = "Reg_{0}";
            public const string PasswordReset = "Reset_{0}";
            public const string EmailChange = "EmailChg_{0}";
            public const string RefreshToken = "RT_{0}";

            public const int OtpExpirationMinutes = 15;
        }
    }
}
