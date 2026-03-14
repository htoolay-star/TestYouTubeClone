using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.DTOs.Cache
{
    public class OtpCacheItem
    {
        public string Otp { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? NewEmail { get; set; }
    }
}
