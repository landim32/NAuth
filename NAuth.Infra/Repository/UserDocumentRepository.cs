using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.DTO.Document;
using NAuth.Infra.Context;
using NAuth.Infra.Interfaces.Repository;
using System.Collections.Generic;
using System.Linq;

namespace NAuth.Infra.Repository
{
    public class UserDocumentRepository : IUserDocumentRepository<IUserDocumentModel, IUserDocumentDomainFactory>
    {
        private NAuthContext _ccsContext;

        public UserDocumentRepository(NAuthContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private IUserDocumentModel DbToModel(IUserDocumentDomainFactory factory, UserDocument row)
        {
            var md = factory.BuildUserDocumentModel();
            md.DocumentId = row.DocumentId;
            md.UserId = row.UserId;
            md.DocumentType = (DocumentTypeEnum)row.DocumentType;
            md.Base64 = row.Base64;
            return md;
        }

        private void ModelToDb(IUserDocumentModel md, UserDocument row)
        {
            row.DocumentId = md.DocumentId;
            row.UserId = md.UserId;
            row.DocumentType = (int)md.DocumentType;
            row.Base64 = md.Base64;
        }

        public IUserDocumentModel Insert(IUserDocumentModel model, IUserDocumentDomainFactory factory)
        {
            var row = new UserDocument();
            ModelToDb(model, row);
            _ccsContext.Add(row);
            _ccsContext.SaveChanges();
            model.DocumentId = row.DocumentId;
            return model;
        }

        public IUserDocumentModel Update(IUserDocumentModel model, IUserDocumentDomainFactory factory)
        {
            var row = _ccsContext.UserDocuments.Find(model.DocumentId);
            ModelToDb(model, row);
            _ccsContext.UserDocuments.Update(row);
            _ccsContext.SaveChanges();
            return model;
        }

        public void Delete(long documentId)
        {
            var row = _ccsContext.UserDocuments.Find(documentId);
            if (row == null)
                return;
            _ccsContext.UserDocuments.Remove(row);
            _ccsContext.SaveChanges();
        }

        public IEnumerable<IUserDocumentModel> ListByUser(long userId, IUserDocumentDomainFactory factory)
        {
            return _ccsContext.UserDocuments
                .Where(x => x.UserId == userId)
                .Select(x => DbToModel(factory, x));
        }
    }
}
