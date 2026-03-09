using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class ContentReport
{
    public long ReportId { get; set; }

    public int ReporterUserId { get; set; }

    public long? VideoId { get; set; }

    public long? CommentId { get; set; }

    public byte ReasonId { get; set; }

    public string? Details { get; set; }

    public byte? Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ReportReason Reason { get; set; } = null!;

    public virtual User ReporterUser { get; set; } = null!;
}
