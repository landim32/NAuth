using NAuth.Domain.Factory.Interfaces;
using NAuth.DTO.Document;
using System.Collections.Generic;

namespace NAuth.Domain.Models.Models
{
    public interface IUserDocumentModel
    {
        long DocumentId { get; set; }

        long? UserId { get; set; }

        DocumentTypeEnum DocumentType { get; set; }

        string Base64 { get; set; }

        IEnumerable<IUserDocumentModel> ListByUser(long userId, IUserDocumentDomainFactory factory);
        IUserDocumentModel Insert(IUserDocumentModel model, IUserDocumentDomainFactory factory);
        IUserDocumentModel Update(IUserDocumentModel model, IUserDocumentDomainFactory factory);
        void Delete(long documentId);
    }
}
