namespace NAuth.Infra.Interfaces.Repository
{
    public interface IUserAddressRepository<TModel, TFactory>
    {
        IEnumerable<TModel> ListByUser(long userId, TFactory factory);
        TModel Insert(TModel model, TFactory factory);
        void DeleteAllByUser(long userId);
    }
}
