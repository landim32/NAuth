using NAuth.Domain.Factory.Interfaces;
using System.Collections.Generic;

namespace NAuth.Domain.Models.Models
{
    public interface IUserAddressModel
    {
        long AddressId { get; set; }

        public long UserId { get; set; }

        string ZipCode { get; set; }

        string Address { get; set; }

        string Complement { get; set; }

        string Neighborhood { get; set; }

        string City { get; set; }

        string State { get; set; }

        IEnumerable<IUserAddressModel> ListByUser(long userId, IUserAddressDomainFactory factory);
        IUserAddressModel Insert(IUserAddressDomainFactory factory);
        void DeleteAllByUser(long userId);
    }
}
