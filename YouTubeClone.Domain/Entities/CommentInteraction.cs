using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class CommentInteraction
{
    public int UserId { get; set; }

    public long CommentId { get; set; }

    public byte ReactionType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Comment Comment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
