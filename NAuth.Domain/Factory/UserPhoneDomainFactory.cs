using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;

namespace NAuth.Domain.Factory
{
    public class UserPhoneDomainFactory : IUserPhoneDomainFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserPhoneRepository<IUserPhoneModel, IUserPhoneDomainFactory> _repositoryPhone;

        public UserPhoneDomainFactory(IUnitOfWork unitOfWork, IUserPhoneRepository<IUserPhoneModel, IUserPhoneDomainFactory> repositoryPhone)
        {
            _unitOfWork = unitOfWork;
            _repositoryPhone = repositoryPhone;
        }

        public IUserPhoneModel BuildUserPhoneModel()
        {
            return new UserPhoneModel(_unitOfWork, _repositoryPhone);
        }
    }
}
