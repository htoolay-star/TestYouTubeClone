using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.DTOs
{
    public record AuthResponse(
        string Token,
        string Username,
        string Email,
        string Role,
        DateTime Expiration
    );
}
