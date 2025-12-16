using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;
using System.Collections.Generic;
using System.Linq;

namespace NAuth.Domain.Models
{
    public class UserPhoneModel : IUserPhoneModel
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IUserPhoneRepository<IUserPhoneModel, IUserPhoneDomainFactory> _repositoryPhone;

        public UserPhoneModel(IUnitOfWork unitOfWork, IUserPhoneRepository<IUserPhoneModel, IUserPhoneDomainFactory> repositoryPhone)
        {
            _unitOfWork = unitOfWork;
            _repositoryPhone = repositoryPhone;
        }

        public long PhoneId { get; set; }
        public long UserId { get; set; }
        public string Phone { get; set; }

        public void DeleteAllByUser(long userId)
        {
            _repositoryPhone.DeleteAllByUser(userId);
        }

        public IUserPhoneModel Insert(IUserPhoneDomainFactory factory)
        {
            return _repositoryPhone.Insert(this, factory);
        }

        public IEnumerable<IUserPhoneModel> ListByUser(long userId, IUserPhoneDomainFactory factory)
        {
            return _repositoryPhone.ListByUser(userId, factory).ToList();
        }
    }
}
