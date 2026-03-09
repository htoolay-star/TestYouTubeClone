using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class Subscription
{
    public int SubscriberId { get; set; }

    public int ChannelId { get; set; }

    public byte NotificationLevel { get; set; }

    public DateTime SubscribedAt { get; set; }

    public virtual Channel Channel { get; set; } = null!;

    public virtual User Subscriber { get; set; } = null!;
}
