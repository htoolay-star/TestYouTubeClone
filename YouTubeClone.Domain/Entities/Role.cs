using System;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Entities;

public partial class Role
{
    public byte RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? NormalizedName { get; set; }

    public string? Description { get; set; }

    public bool IsSystemRole { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
