using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;

namespace NAuth.Domain.Factory
{
    public class UserAddressDomainFactory : IUserAddressDomainFactory
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory> _repositoryAddr;

        public UserAddressDomainFactory(IUnitOfWork unitOfWork, IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory> repositoryAddr)
        {
            _unitOfWork = unitOfWork;
            _repositoryAddr = repositoryAddr;
        }
        public IUserAddressModel BuildUserAddressModel()
        {
            return new UserAddressModel(_unitOfWork, _repositoryAddr);
        }
    }
}
