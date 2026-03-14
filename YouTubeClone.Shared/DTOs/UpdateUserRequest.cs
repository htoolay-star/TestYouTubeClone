using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.DTOs
{
    public record UpdateUserRequest(
    string? Username,
    string? Bio,
    string? ProfilePictureUrl,
    DateOnly? DateOfBirth,
    byte? Gender           
);
}
