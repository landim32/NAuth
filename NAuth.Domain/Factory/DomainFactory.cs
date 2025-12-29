using NAuth.Domain.Factory.Interfaces;

namespace NAuth.Domain.Factory
{
    public class DomainFactory
    {
        public IUserDomainFactory UserFactory { get; }
        public IUserPhoneDomainFactory PhoneFactory { get; }
        public IUserAddressDomainFactory AddressFactory { get; }
        public IRoleDomainFactory RoleFactory { get; }

        public DomainFactory(
            IUserDomainFactory userFactory,
            IUserPhoneDomainFactory phoneFactory,
            IUserAddressDomainFactory addressFactory,
            IRoleDomainFactory roleFactory)
        {
            UserFactory = userFactory;
            PhoneFactory = phoneFactory;
            AddressFactory = addressFactory;
            RoleFactory = roleFactory;
        }
    }
}
