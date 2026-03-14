using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.DTOs
{
    public record ChangeEmailRequest(
        string NewEmail,
        string ConfirmEmail,
        string Password
    );
}
