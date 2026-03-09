using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class Channel
{
    public int ChannelId { get; set; }

    public Guid PublicId { get; set; }

    public int UserId { get; set; }

    public string ChannelName { get; set; } = null!;

    public string Handle { get; set; } = null!;

    public string? NormalizedHandle { get; set; }

    public string? Description { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? CoverImageUrl { get; set; }

    public bool IsVerified { get; set; }

    public string? CountryCode { get; set; }

    public byte ChannelStatus { get; set; }

    public int SubscriberCount { get; set; }

    public int VideoCount { get; set; }

    public long TotalViews { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<ChannelHandleHistory> ChannelHandleHistories { get; set; } = new List<ChannelHandleHistory>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
