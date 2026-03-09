using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class Video
{
    public long VideoId { get; set; }

    public Guid PublicId { get; set; }

    public int ChannelId { get; set; }

    public byte CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string VideoUrl { get; set; } = null!;

    public string ThumbnailUrl { get; set; } = null!;

    public int DurationInSeconds { get; set; }

    public byte VisibilityStatus { get; set; }

    public byte ProcessingStatus { get; set; }

    public long ViewsCount { get; set; }

    public long LikesCount { get; set; }

    public int CommentsCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Channel Channel { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<PlaylistItem> PlaylistItems { get; set; } = new List<PlaylistItem>();

    public virtual ICollection<VideoInteraction> VideoInteractions { get; set; } = new List<VideoInteraction>();

    public virtual ICollection<VideoViewHistory> VideoViewHistories { get; set; } = new List<VideoViewHistory>();

    public virtual ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
