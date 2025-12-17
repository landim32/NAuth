using System;
using System.Collections.Generic;

namespace NAuth.Infra.Context;

public partial class Role
{
    public long RoleId { get; set; }

    public string Slug { get; set; }

    public string Name { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
