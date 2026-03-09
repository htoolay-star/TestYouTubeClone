using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class Playlist
{
    public long PlaylistId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public byte VisibilityStatus { get; set; }

    public bool IsSystemGenerated { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<PlaylistItem> PlaylistItems { get; set; } = new List<PlaylistItem>();

    public virtual User User { get; set; } = null!;
}
