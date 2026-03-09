using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using YouTubeClone.Domain.Entities;

namespace YouTubeClone.Domain.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Channel> Channels { get; set; }

    public virtual DbSet<ChannelHandleHistory> ChannelHandleHistories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CommentInteraction> CommentInteractions { get; set; }

    public virtual DbSet<CommentMention> CommentMentions { get; set; }

    public virtual DbSet<ContentReport> ContentReports { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationSetting> NotificationSettings { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<PlaylistItem> PlaylistItems { get; set; }

    public virtual DbSet<ReportReason> ReportReasons { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SearchHistory> SearchHistories { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    public virtual DbSet<VideoInteraction> VideoInteractions { get; set; }

    public virtual DbSet<VideoViewHistory> VideoViewHistories { get; set; }

    public virtual DbSet<WatchHistory> WatchHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B274A3871");

            entity.ToTable("Categories", "Media");

            entity.HasIndex(e => e.CategoryName, "UQ__Categori__8517B2E022810C14").IsUnique();

            entity.Property(e => e.CategoryId).ValueGeneratedOnAdd();
            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<Channel>(entity =>
        {
            entity.ToTable("Channels", "Media", tb =>
                {
                    tb.HasTrigger("trg_Channels_HandleManager");
                    tb.HasTrigger("trg_Channels_UpdateTimestamp");
                });

            entity.HasIndex(e => e.NormalizedHandle, "UIX_Channels_NormalizedHandle").IsUnique();

            entity.HasIndex(e => e.Handle, "UQ_Channels_Handle").IsUnique();

            entity.HasIndex(e => e.UserId, "UQ_Channels_UserId").IsUnique();

            entity.Property(e => e.ChannelName).HasMaxLength(100);
            entity.Property(e => e.ChannelStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.CountryCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CoverImageUrl).HasMaxLength(2048);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Handle).HasMaxLength(50);
            entity.Property(e => e.NormalizedHandle)
                .HasMaxLength(50)
                .HasComputedColumnSql("(upper([Handle]))", true);
            entity.Property(e => e.PublicId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(2048);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithOne(p => p.Channel)
                .HasForeignKey<Channel>(d => d.UserId)
                .HasConstraintName("FK_Channels_Users");
        });

        modelBuilder.Entity<ChannelHandleHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId);

            entity.ToTable("ChannelHandleHistory", "Media");

            entity.Property(e => e.ChangedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.NewHandle).HasMaxLength(50);
            entity.Property(e => e.OldHandle).HasMaxLength(50);

            entity.HasOne(d => d.Channel).WithMany(p => p.ChannelHandleHistories)
                .HasForeignKey(d => d.ChannelId)
                .HasConstraintName("FK_HandleHistory_Channels");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments", "Media", tb => tb.HasTrigger("trg_Comments_Maintenance"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK_Comments_Parent");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Users");

            entity.HasOne(d => d.Video).WithMany(p => p.Comments)
                .HasForeignKey(d => d.VideoId)
                .HasConstraintName("FK_Comments_Videos");
        });

        modelBuilder.Entity<CommentInteraction>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.CommentId });

            entity.ToTable("CommentInteractions", "Media");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Comment).WithMany(p => p.CommentInteractions)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK_CInteractions_Comments");

            entity.HasOne(d => d.User).WithMany(p => p.CommentInteractions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CInteractions_Users");
        });

        modelBuilder.Entity<CommentMention>(entity =>
        {
            entity.HasKey(e => e.MentionId).HasName("PK__CommentM__5D9162DC769CC036");

            entity.ToTable("CommentMentions", "Media");

            entity.HasOne(d => d.Comment).WithMany(p => p.CommentMentions)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK_Mentions_Comments");

            entity.HasOne(d => d.MentionedUser).WithMany(p => p.CommentMentions)
                .HasForeignKey(d => d.MentionedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mentions_Users");
        });

        modelBuilder.Entity<ContentReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__ContentR__D5BD4805CFB1BACB");

            entity.ToTable("ContentReports", "Social");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasDefaultValue((byte)1);

            entity.HasOne(d => d.Reason).WithMany(p => p.ContentReports)
                .HasForeignKey(d => d.ReasonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reports_Reasons");

            entity.HasOne(d => d.ReporterUser).WithMany(p => p.ContentReports)
                .HasForeignKey(d => d.ReporterUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reports_Users");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1263C9593E");

            entity.ToTable("Notifications", "Social");

            entity.HasIndex(e => new { e.RecipientUserId, e.IsRead }, "IX_Notifications_Recipient_Read");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.ActorUser).WithMany(p => p.NotificationActorUsers)
                .HasForeignKey(d => d.ActorUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Noti_Actor");

            entity.HasOne(d => d.RecipientUser).WithMany(p => p.NotificationRecipientUsers)
                .HasForeignKey(d => d.RecipientUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Noti_Recipient");
        });

        modelBuilder.Entity<NotificationSetting>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Notifica__1788CC4CE050A2B3");

            entity.ToTable("NotificationSettings", "Social");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.ReceiveLikeNoti).HasDefaultValue(true);
            entity.Property(e => e.ReceiveMentionNoti).HasDefaultValue(true);
            entity.Property(e => e.ReceiveNewVideoNoti).HasDefaultValue(true);
            entity.Property(e => e.ReceiveReplyNoti).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithOne(p => p.NotificationSetting)
                .HasForeignKey<NotificationSetting>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NotiSettings_Users");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permissions", "Identity");

            entity.HasIndex(e => e.PermissionName, "UQ_Permissions_PermissionName").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.NormalizedName)
                .HasMaxLength(50)
                .HasComputedColumnSql("(upper([PermissionName]))", true);
            entity.Property(e => e.PermissionName).HasMaxLength(50);
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.PlaylistId).HasName("PK__Playlist__B30167A0E5F4B442");

            entity.ToTable("Playlists", "Media");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.VisibilityStatus).HasDefaultValue((byte)1);

            entity.HasOne(d => d.User).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Playlists_Users");
        });

        modelBuilder.Entity<PlaylistItem>(entity =>
        {
            entity.HasKey(e => new { e.PlaylistId, e.VideoId });

            entity.ToTable("PlaylistItems", "Media");

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Playlist).WithMany(p => p.PlaylistItems)
                .HasForeignKey(d => d.PlaylistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Items_Playlists");

            entity.HasOne(d => d.Video).WithMany(p => p.PlaylistItems)
                .HasForeignKey(d => d.VideoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Items_Videos");
        });

        modelBuilder.Entity<ReportReason>(entity =>
        {
            entity.HasKey(e => e.ReasonId).HasName("PK__ReportRe__A4F8C0E7CBEB7F88");

            entity.ToTable("ReportReasons", "Social");

            entity.Property(e => e.ReasonId).ValueGeneratedOnAdd();
            entity.Property(e => e.ReasonTitle).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles", "Identity");

            entity.HasIndex(e => e.RoleName, "UQ_Roles_RoleName").IsUnique();

            entity.Property(e => e.RoleId).ValueGeneratedOnAdd();
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.NormalizedName)
                .HasMaxLength(20)
                .HasComputedColumnSql("(upper([RoleName]))", true);
            entity.Property(e => e.RoleName).HasMaxLength(20);

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .HasConstraintName("FK_RolePermissions_Permissions"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_RolePermissions_Roles"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId");
                        j.ToTable("RolePermissions", "Identity");
                    });
        });

        modelBuilder.Entity<SearchHistory>(entity =>
        {
            entity.HasKey(e => e.SearchId).HasName("PK__SearchHi__21C535F49D66EC4D");

            entity.ToTable("SearchHistory", "Social");

            entity.HasIndex(e => new { e.UserId, e.SearchedAt }, "IX_SearchHistory_User").IsDescending(false, true);

            entity.Property(e => e.SearchQuery).HasMaxLength(500);
            entity.Property(e => e.SearchedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.SearchHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Search_Users");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => new { e.SubscriberId, e.ChannelId });

            entity.ToTable("Subscriptions", "Media", tb => tb.HasTrigger("trg_Subscriptions_Maintenance"));

            entity.Property(e => e.NotificationLevel).HasDefaultValue((byte)1);
            entity.Property(e => e.SubscribedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Channel).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.ChannelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subs_Channels");

            entity.HasOne(d => d.Subscriber).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.SubscriberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subs_Users");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__Tags__657CF9AC1A30B809");

            entity.ToTable("Tags", "Media");

            entity.HasIndex(e => e.TagName, "UQ__Tags__BDE0FD1DB01DBC64").IsUnique();

            entity.Property(e => e.NormalizedTagName)
                .HasMaxLength(50)
                .HasComputedColumnSql("(upper([TagName]))", true);
            entity.Property(e => e.TagName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "Identity", tb => tb.HasTrigger("trg_Users_UpdateTimestamp"));

            entity.HasIndex(e => e.PublicId, "IX_Users_PublicId");

            entity.HasIndex(e => e.NormalizedEmail, "UIX_Users_NormalizedEmail")
                .IsUnique()
                .HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.NormalizedUsername, "UIX_Users_NormalizedUsername")
                .IsUnique()
                .HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.PublicId, "UQ_Users_PublicId").IsUnique();

            entity.Property(e => e.AccountStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsSubscriptionPublic).HasDefaultValue(true);
            entity.Property(e => e.NormalizedEmail)
                .HasMaxLength(100)
                .HasComputedColumnSql("(upper([Email]))", true);
            entity.Property(e => e.NormalizedUsername)
                .HasMaxLength(50)
                .HasComputedColumnSql("(upper([Username]))", true);
            entity.Property(e => e.ProfilePictureUrl).HasMaxLength(2048);
            entity.Property(e => e.PublicId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.ToTable("UserRoles", "Identity");

            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserRoles_Users");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.ToTable("Videos", "Media", tb => tb.HasTrigger("trg_Videos_UpdateChannelCount"));

            entity.HasIndex(e => new { e.CategoryId, e.VisibilityStatus, e.ProcessingStatus }, "IX_Videos_Category_Stats");

            entity.HasIndex(e => new { e.CreatedAt, e.VisibilityStatus, e.ProcessingStatus }, "IX_Videos_Trending");

            entity.HasIndex(e => e.PublicId, "UIX_Videos_PublicId").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ProcessingStatus).HasDefaultValue((byte)1);
            entity.Property(e => e.PublicId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(2048);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.VideoUrl).HasMaxLength(2048);
            entity.Property(e => e.VisibilityStatus).HasDefaultValue((byte)1);

            entity.HasOne(d => d.Category).WithMany(p => p.Videos)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Videos_Categories");

            entity.HasOne(d => d.Channel).WithMany(p => p.Videos)
                .HasForeignKey(d => d.ChannelId)
                .HasConstraintName("FK_Videos_Channels");

            entity.HasMany(d => d.Tags).WithMany(p => p.Videos)
                .UsingEntity<Dictionary<string, object>>(
                    "VideoTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK_VideoTags_Tags"),
                    l => l.HasOne<Video>().WithMany()
                        .HasForeignKey("VideoId")
                        .HasConstraintName("FK_VideoTags_Videos"),
                    j =>
                    {
                        j.HasKey("VideoId", "TagId").HasName("PK__VideoTag__6CB2DDF0D94EED74");
                        j.ToTable("VideoTags", "Media");
                    });
        });

        modelBuilder.Entity<VideoInteraction>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.VideoId });

            entity.ToTable("VideoInteractions", "Media");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.VideoInteractions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VInteractions_Users");

            entity.HasOne(d => d.Video).WithMany(p => p.VideoInteractions)
                .HasForeignKey(d => d.VideoId)
                .HasConstraintName("FK_VInteractions_Videos");
        });

        modelBuilder.Entity<VideoViewHistory>(entity =>
        {
            entity.HasKey(e => e.ViewId).HasName("PK__VideoVie__1E371CF69B130746");

            entity.ToTable("VideoViewHistory", "Media");

            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.ViewedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Video).WithMany(p => p.VideoViewHistories)
                .HasForeignKey(d => d.VideoId)
                .HasConstraintName("FK_ViewHistory_Videos");
        });

        modelBuilder.Entity<WatchHistory>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.VideoId });

            entity.ToTable("WatchHistory", "Media");

            entity.Property(e => e.IsFinished).HasComputedColumnSql("(case when [LastWatchedPosition]>=[TotalDuration]*(0.9) then (1) else (0) end)", false);
            entity.Property(e => e.LastWatchedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.WatchHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_History_Users");

            entity.HasOne(d => d.Video).WithMany(p => p.WatchHistories)
                .HasForeignKey(d => d.VideoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_History_Videos");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
