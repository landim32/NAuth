using System;
using System.Collections.Generic;

namespace DB.Infra.Context;

public partial class UserToken
{
    public long TokenId { get; set; }

    public long UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastAccess { get; set; }

    public DateTime ExpireAt { get; set; }

    public string Fingerprint { get; set; }

    public string IpAddress { get; set; }

    public string Token { get; set; }

    public string UserAgent { get; set; }

    public virtual User User { get; set; }
}
