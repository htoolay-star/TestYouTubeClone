using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class Tag
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;

    public string? NormalizedTagName { get; set; }

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
