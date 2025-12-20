namespace NAuth.Infra.Interfaces.Repository
{
    public interface IUserRepository<TModel, TFactory>
    {
        TModel Insert(TModel model, TFactory factory);
        TModel Update(TModel model, TFactory factory);
        IEnumerable<TModel> ListUsers(TFactory factory);
        IEnumerable<TModel> SearchUsers(string searchTerm, int page, int pageSize, out int totalCount, TFactory factory);
        TModel GetById(long userId, TFactory factory);
        TModel GetByEmail(string email, TFactory factory);
        TModel GetBySlug(string slug, TFactory factory);
        TModel GetByStripeId(string stripeId, TFactory factory);
        TModel LoginWithEmail(string email, string encryptPwd, TFactory factory);
        TModel GetUserByRecoveryHash(string recoveryHash, TFactory factory);
        void UpdateRecoveryHash(long userId, string recoveryHash);
        void ChangePassword(long userId, string encryptPwd);
        bool HasPassword(long userId, TFactory factory);
        bool ExistSlug(long userId, string slug);
        string GetHashedPassword(long userId);
    }
}
