using Core.Domain.Repository;
using DB.Infra.Context;
using NAuth.Domain.Interfaces.Factory;
using NAuth.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infra.Repository
{
    public class UserTokenRepository : IUserTokenRepository<IUserTokenModel, IUserTokenDomainFactory>
    {
        private NAuthContext _ccsContext;

        public UserTokenRepository(NAuthContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private IUserTokenModel DbToModel(IUserTokenDomainFactory factory, UserToken row)
        {
            var md = factory.BuildUserTokenModel();
            md.TokenId = row.TokenId;
            md.UserId = row.UserId;
            md.CreatedAt = row.CreatedAt;
            md.LastAccess = row.LastAccess;
            md.ExpireAt = row.ExpireAt;
            md.Fingerprint = row.Fingerprint;
            md.IpAddress = row.IpAddress;
            md.Token = row.Token;
            md.UserAgent = row.UserAgent;
            return md;
        }

        private void ModelToDb(IUserTokenModel md, UserToken row)
        {
            row.TokenId = md.TokenId;
            row.UserId = md.UserId;
            row.CreatedAt = md.CreatedAt;
            row.LastAccess = md.LastAccess;
            row.ExpireAt = md.ExpireAt;
            row.Fingerprint = md.Fingerprint;
            row.IpAddress = md.IpAddress;
            row.Token = md.Token;
            row.UserAgent = md.UserAgent;
        }

        public IEnumerable<IUserTokenModel> ListByUser(long userId, IUserTokenDomainFactory factory)
        {
            var rows = _ccsContext.UserTokens.Where(x => x.UserId == userId).OrderBy(x => x.CreatedAt).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public IUserTokenModel GetById(long tokenId, IUserTokenDomainFactory factory)
        {
            var row = _ccsContext.UserTokens.Find(tokenId);
            if (row == null)
                return null;
            return DbToModel(factory, row);
        }
        public IUserTokenModel GetByToken(string token, IUserTokenDomainFactory factory)
        {
            var row = _ccsContext.UserTokens
                .FirstOrDefault(x => x.Token == token && x.ExpireAt > DateTime.Now);
            if (row == null)
                return null;
            return DbToModel(factory, row);
        }

        public IUserTokenModel Insert(IUserTokenModel model, IUserTokenDomainFactory factory)
        {
            var row = new UserToken();
            ModelToDb(model, row);
            _ccsContext.Add(row);
            _ccsContext.SaveChanges();
            model.TokenId = row.TokenId;
            return model;
        }

        public IUserTokenModel Update(IUserTokenModel model, IUserTokenDomainFactory factory)
        {
            var row = _ccsContext.UserTokens.Find(model.TokenId);
            if (row == null)
            {
                throw new Exception($"Token with ID {model.TokenId} not found.");
            }
            ModelToDb(model, row);
            _ccsContext.UserTokens.Update(row);
            _ccsContext.SaveChanges();
            return model;
        }
    }
}
