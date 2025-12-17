using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Context;
using NAuth.Infra.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NAuth.Infra.Repository
{
    public class UserAddressRepository : IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory>
    {
        protected readonly NAuthContext _ccsContext;

        public UserAddressRepository(NAuthContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private static IUserAddressModel DbToModel(IUserAddressDomainFactory factory, UserAddress row)
        {
            var md = factory.BuildUserAddressModel();
            md.AddressId = row.AddressId;
            md.UserId = row.UserId;
            md.ZipCode = row.ZipCode;
            md.Address = row.Address;
            md.Complement = row.Complement;
            md.Neighborhood = row.Neighborhood;
            md.City = row.City;
            md.State = row.State;
            return md;
        }

        private static void ModelToDb(IUserAddressModel md, UserAddress row)
        {
            row.AddressId = md.AddressId;
            row.UserId = md.UserId;
            row.ZipCode = md.ZipCode;
            row.Address = md.Address;
            row.Complement = md.Complement;
            row.Neighborhood = md.Neighborhood;
            row.City = md.City;
            row.State = md.State;
        }

        public IUserAddressModel Insert(IUserAddressModel model, IUserAddressDomainFactory factory)
        {
            var row = new UserAddress();
            ModelToDb(model, row);
            _ccsContext.Add(row);
            _ccsContext.SaveChanges();
            model.AddressId = row.AddressId;
            return model;
        }

        public void DeleteAllByUser(long userId)
        {
            var rows = _ccsContext.UserAddresses.Where(x => x.UserId == userId);
            if (rows.Count() == 0)
                return;
            _ccsContext.UserAddresses.RemoveRange(rows);
            _ccsContext.SaveChanges();
        }

        public IEnumerable<IUserAddressModel> ListByUser(long userId, IUserAddressDomainFactory factory)
        {
            var addrs = _ccsContext.UserAddresses.Where(x => x.UserId == userId).ToList();
            return addrs.Select(x => DbToModel(factory, x));
        }
    }
}
