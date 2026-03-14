using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.DTOs
{
    public record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword,
        string ConfirmPassword
    );
}
