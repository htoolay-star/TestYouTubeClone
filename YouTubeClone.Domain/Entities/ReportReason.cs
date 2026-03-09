using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class ReportReason
{
    public byte ReasonId { get; set; }

    public string ReasonTitle { get; set; } = null!;

    public virtual ICollection<ContentReport> ContentReports { get; set; } = new List<ContentReport>();
}
