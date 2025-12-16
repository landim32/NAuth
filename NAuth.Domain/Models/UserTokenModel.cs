using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;
using System;
using System.Collections.Generic;

namespace NAuth.Domain.Models
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
