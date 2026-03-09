using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class WatchHistory
{
    public int UserId { get; set; }

    public long VideoId { get; set; }

    public int LastWatchedPosition { get; set; }

    public int TotalDuration { get; set; }

    public DateTime LastWatchedAt { get; set; }

    public int IsFinished { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Video Video { get; set; } = null!;
}
