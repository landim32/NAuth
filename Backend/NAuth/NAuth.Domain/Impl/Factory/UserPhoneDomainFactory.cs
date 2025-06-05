using Core.Domain.Repository;
using Core.Domain;
using NAuth.Domain.Interfaces.Factory;
using NAuth.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAuth.Domain.Impl.Models;

namespace NAuth.Domain.Impl.Factory
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
