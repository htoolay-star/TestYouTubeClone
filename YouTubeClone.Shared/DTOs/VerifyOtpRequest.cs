using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.DTOs
{
    public record VerifyOtpRequest
    (
        Guid PublicId,
        string Otp
    );
}
