using NAuth.Domain.Factory.Interfaces;
using System.Collections.Generic;

namespace NAuth.Domain.Models.Models
{
    public interface IUserPhoneModel
    {
        long PhoneId { get; set; }

        long UserId { get; set; }

        string Phone { get; set; }

        IEnumerable<IUserPhoneModel> ListByUser(long userId, IUserPhoneDomainFactory factory);
        IUserPhoneModel Insert(IUserPhoneDomainFactory factory);
        void DeleteAllByUser(long userId);
    }
}
