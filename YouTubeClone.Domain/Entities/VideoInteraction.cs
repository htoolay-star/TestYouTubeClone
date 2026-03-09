using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class VideoInteraction
{
    public int UserId { get; set; }

    public long VideoId { get; set; }

    public byte ReactionType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Video Video { get; set; } = null!;
}
