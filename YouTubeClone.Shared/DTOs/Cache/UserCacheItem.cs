using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.DTOs.Cache
{
    public class UserCacheItem
    {
        public int UserId { get; set; }
        public Guid PublicId { get; set; }

        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ProfilePictureUrl { get; set; }

        public bool IsEmailVerified { get; set; }
        public byte AccountStatus { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastLoginAt { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
