using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class CommentMention
{
    public long MentionId { get; set; }

    public long CommentId { get; set; }

    public int MentionedUserId { get; set; }

    public virtual Comment Comment { get; set; } = null!;

    public virtual User MentionedUser { get; set; } = null!;
}
