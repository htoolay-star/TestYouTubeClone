using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class NotificationSetting
{
    public int UserId { get; set; }

    public bool ReceiveNewVideoNoti { get; set; }

    public bool ReceiveMentionNoti { get; set; }

    public bool ReceiveReplyNoti { get; set; }

    public bool ReceiveLikeNoti { get; set; }

    public virtual User User { get; set; } = null!;
}
