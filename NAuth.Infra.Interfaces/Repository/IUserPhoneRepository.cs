namespace NAuth.Infra.Interfaces.Repository
{
    public interface IUserPhoneRepository<TModel, TFactory>
    {
        IEnumerable<TModel> ListByUser(long userId, TFactory factory);
        TModel Insert(TModel model, TFactory factory);
        void DeleteAllByUser(long userId);
    }
}
