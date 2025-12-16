namespace NAuth.Infra.Interfaces.Repository
{
    public interface IUserDocumentRepository<TModel, TFactory>
    {
        IEnumerable<TModel> ListByUser(long userId, TFactory factory);
        TModel Insert(TModel model, TFactory factory);
        TModel Update(TModel model, TFactory factory);
        void Delete(long documentId);
    }
}
