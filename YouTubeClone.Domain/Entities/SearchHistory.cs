using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class SearchHistory
{
    public long SearchId { get; set; }

    public int UserId { get; set; }

    public string SearchQuery { get; set; } = null!;

    public DateTime SearchedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
