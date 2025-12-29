namespace NAuth.Infra.Interfaces.Repository
{
    public interface IRoleRepository<TModel, TFactory>
    {
        TModel Insert(TModel model, TFactory factory);
        TModel Update(TModel model, TFactory factory);
        TModel GetById(long roleId, TFactory factory);
        TModel GetBySlug(string slug, TFactory factory);
        IEnumerable<TModel> ListRoles(TFactory factory);
        bool ExistSlug(long roleId, string slug);
        void Delete(long roleId);
    }
}
