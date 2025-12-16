namespace NAuth.Infra.Interfaces.Repository
{
    public interface IUserTokenRepository<TModel, TFactory>
    {
        IEnumerable<TModel> ListByUser(long userId, TFactory factory);
        TModel GetById(long tokenId, TFactory factory);
        TModel GetByToken(string token, TFactory factory);
        TModel Insert(TModel model, TFactory factory);
        TModel Update(TModel model, TFactory factory);
    }
}
