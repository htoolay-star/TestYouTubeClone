using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class Notification
{
    public long NotificationId { get; set; }

    public int RecipientUserId { get; set; }

    public int ActorUserId { get; set; }

    public byte NotificationType { get; set; }

    public long ReferenceId { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User ActorUser { get; set; } = null!;

    public virtual User RecipientUser { get; set; } = null!;
}
