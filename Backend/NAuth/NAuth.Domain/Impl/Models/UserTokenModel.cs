using Core.Domain;
using Core.Domain.Repository;
using NAuth.Domain.Interfaces.Factory;
using NAuth.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAuth.Domain.Impl.Models
{
    public class UserTokenModel : IUserTokenModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserTokenRepository<IUserTokenModel, IUserTokenDomainFactory> _repositoryToken;

        public UserTokenModel(IUnitOfWork unitOfWork, IUserTokenRepository<IUserTokenModel, IUserTokenDomainFactory> repositoryToken)
        {
            _unitOfWork = unitOfWork;
            _repositoryToken = repositoryToken;
        }

        public long TokenId { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastAccess { get; set; }
        public DateTime ExpireAt { get; set; }
        public string Fingerprint { get; set; }
        public string IpAddress { get; set; }
        public string Token { get; set; }
        public string UserAgent { get; set; }

        public IUserTokenModel GetById(long tokenId, IUserTokenDomainFactory factory)
        {
            return _repositoryToken.GetById(tokenId, factory);
        }

        public IUserTokenModel GetByToken(string token, IUserTokenDomainFactory factory)
        {
            return _repositoryToken.GetByToken(token, factory);
        }

        public IUserTokenModel Insert(IUserTokenDomainFactory factory)
        {
            return _repositoryToken.Insert(this, factory);
        }

        public IEnumerable<IUserTokenModel> ListByUser(long userId, IUserTokenDomainFactory factory)
        {
            return _repositoryToken.ListByUser(userId, factory);
        }

        public IUserTokenModel Update(IUserTokenDomainFactory factory)
        {
            return _repositoryToken.Update(this, factory);
        }
    }
}
