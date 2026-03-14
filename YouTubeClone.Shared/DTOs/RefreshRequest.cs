using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.DTOs
{
    public record RefreshRequest(
        string AccessToken,
        string RefreshToken
    );
}
