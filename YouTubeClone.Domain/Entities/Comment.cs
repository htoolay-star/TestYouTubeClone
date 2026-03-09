using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class Comment
{
    public long CommentId { get; set; }

    public long VideoId { get; set; }

    public int UserId { get; set; }

    public long? ParentCommentId { get; set; }

    public string CommentText { get; set; } = null!;

    public int LikesCount { get; set; }

    public int DislikesCount { get; set; }

    public int ReplyCount { get; set; }

    public bool IsEdited { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<CommentInteraction> CommentInteractions { get; set; } = new List<CommentInteraction>();

    public virtual ICollection<CommentMention> CommentMentions { get; set; } = new List<CommentMention>();

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    public virtual Comment? ParentComment { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Video Video { get; set; } = null!;
}
