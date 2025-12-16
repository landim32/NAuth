using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Context;
using NAuth.Infra.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NAuth.Infra.Repository
{
    public class UserPhoneRepository : IUserPhoneRepository<IUserPhoneModel, IUserPhoneDomainFactory>
    {
        private NAuthContext _ccsContext;

        public UserPhoneRepository(NAuthContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private IUserPhoneModel DbToModel(IUserPhoneDomainFactory factory, UserPhone row)
        {
            var md = factory.BuildUserPhoneModel();
            md.PhoneId = row.PhoneId;
            md.UserId = row.UserId;
            md.Phone = row.Phone;
            return md;
        }

        private void ModelToDb(IUserPhoneModel md, UserPhone row)
        {
            row.PhoneId = md.PhoneId;
            row.UserId = md.UserId;
            row.Phone = md.Phone;
        }

        public IUserPhoneModel Insert(IUserPhoneModel model, IUserPhoneDomainFactory factory)
        {
            var row = new UserPhone();
            ModelToDb(model, row);
            _ccsContext.Add(row);
            _ccsContext.SaveChanges();
            model.PhoneId = row.PhoneId;
            return model;
        }

        public void DeleteAllByUser(long userId)
        {
            var rows = _ccsContext.UserPhones.Where(x => x.UserId == userId).ToList();
            if (rows.Count() == 0)
                return;
            _ccsContext.UserPhones.RemoveRange(rows);
            _ccsContext.SaveChanges();
        }

        public IEnumerable<IUserPhoneModel> ListByUser(long userId, IUserPhoneDomainFactory factory)
        {
            var phones = _ccsContext.UserPhones
                .Where(x => x.UserId == userId)
                .ToList();
            return phones.Select(x => DbToModel(factory, x));
        }
    }
}
