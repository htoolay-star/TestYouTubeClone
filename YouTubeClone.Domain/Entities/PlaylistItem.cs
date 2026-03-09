using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class PlaylistItem
{
    public long PlaylistId { get; set; }

    public long VideoId { get; set; }

    public int SortOrder { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual Playlist Playlist { get; set; } = null!;

    public virtual Video Video { get; set; } = null!;
}
