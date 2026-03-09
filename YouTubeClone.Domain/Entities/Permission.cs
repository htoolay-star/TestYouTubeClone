using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class Permission
{
    public int PermissionId { get; set; }

    public string PermissionName { get; set; } = null!;

    public string? NormalizedName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
