using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class ChannelHandleHistory
{
    public int HistoryId { get; set; }

    public int ChannelId { get; set; }

    public string OldHandle { get; set; } = null!;

    public string NewHandle { get; set; } = null!;

    public DateTime ChangedAt { get; set; }

    public virtual Channel Channel { get; set; } = null!;
}
