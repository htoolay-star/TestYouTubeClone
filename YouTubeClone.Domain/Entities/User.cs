using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class User
{
    public int UserId { get; set; }

    public Guid PublicId { get; set; }

    public string Username { get; set; } = null!;

    public string? NormalizedUsername { get; set; }

    public string Email { get; set; } = null!;

    public string? NormalizedEmail { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? Bio { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public byte Gender { get; set; }

    public byte AccountStatus { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public bool IsSubscriptionPublic { get; set; }

    public bool IsWatchHistoryPaused { get; set; }

    public bool IsEmailVerified { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int AccessFailedCount { get; set; }
    public DateTime? LockoutEnd { get; set; }

    public virtual Channel? Channel { get; set; }

    public virtual ICollection<CommentInteraction> CommentInteractions { get; set; } = new List<CommentInteraction>();

    public virtual ICollection<CommentMention> CommentMentions { get; set; } = new List<CommentMention>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<ContentReport> ContentReports { get; set; } = new List<ContentReport>();

    public virtual ICollection<Notification> NotificationActorUsers { get; set; } = new List<Notification>();

    public virtual ICollection<Notification> NotificationRecipientUsers { get; set; } = new List<Notification>();

    public virtual NotificationSetting? NotificationSetting { get; set; }

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();

    public virtual ICollection<SearchHistory> SearchHistories { get; set; } = new List<SearchHistory>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<VideoInteraction> VideoInteractions { get; set; } = new List<VideoInteraction>();

    public virtual ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();
}
