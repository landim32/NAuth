using NAuth.Domain.Models.Models;

namespace NAuth.Domain.Factory.Interfaces
{
    public interface IUserAddressDomainFactory
    {
        IUserAddressModel BuildUserAddressModel();
    }
}
