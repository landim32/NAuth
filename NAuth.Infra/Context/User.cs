using System;
using System.Collections.Generic;

namespace NAuth.Infra.Context;

public partial class User
{
    public long UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string Hash { get; set; }

    public string Slug { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public bool IsAdmin { get; set; }

    public string RecoveryHash { get; set; }

    public string IdDocument { get; set; }

    public DateTime? BirthDate { get; set; }

    public string PixKey { get; set; }

    public string StripeId { get; set; }

    public string Image { get; set; }

    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

    public virtual ICollection<UserDocument> UserDocuments { get; set; } = new List<UserDocument>();

    public virtual ICollection<UserPhone> UserPhones { get; set; } = new List<UserPhone>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
