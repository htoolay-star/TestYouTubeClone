using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.DTOs
{
    public record RegisterRequest(
        string Username,
        string Email,
        string Password,
        string ConfirmPassword,
        string? Bio = null,
        DateOnly? DateOfBirth = null,
        byte Gender = 0
    );
}
