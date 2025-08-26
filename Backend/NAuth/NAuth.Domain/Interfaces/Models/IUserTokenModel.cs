using NAuth.Domain.Interfaces.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAuth.Domain.Interfaces.Models
{
    public interface IUserTokenModel
    {
        long TokenId { get; set; }

        long UserId { get; set; }

        DateTime CreatedAt { get; set; }

        DateTime LastAccess { get; set; }

        DateTime ExpireAt { get; set; }

        string Fingerprint { get; set; }

        string IpAddress { get; set; }

        string Token { get; set; }

        string UserAgent { get; set; }

        IEnumerable<IUserTokenModel> ListByUser(long userId, IUserTokenDomainFactory factory);
        IUserTokenModel GetById(long tokenId, IUserTokenDomainFactory factory);
        IUserTokenModel GetByToken(string token, IUserTokenDomainFactory factory);
        IUserTokenModel Insert(IUserTokenDomainFactory factory);
        IUserTokenModel Update(IUserTokenDomainFactory factory);
    }
}
