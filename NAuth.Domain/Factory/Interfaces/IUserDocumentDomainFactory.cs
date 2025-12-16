using NAuth.Domain.Models.Models;

namespace NAuth.Domain.Factory.Interfaces
{
    public interface IUserDocumentDomainFactory
    {
        IUserDocumentModel BuildUserDocumentModel();
    }
}
