using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class VideoViewHistory
{
    public long ViewId { get; set; }

    public long VideoId { get; set; }

    public int? UserId { get; set; }

    public string? IpAddress { get; set; }

    public DateTime? ViewedAt { get; set; }

    public virtual Video Video { get; set; } = null!;
}
