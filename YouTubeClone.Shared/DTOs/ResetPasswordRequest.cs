using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.DTOs
{
    public record ResetPasswordRequest(
        string Email,
        string Otp,
        string NewPassword
        );
}
