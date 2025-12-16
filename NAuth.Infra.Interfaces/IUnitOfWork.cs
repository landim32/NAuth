namespace NAuth.Infra.Interfaces
{
    public interface IUnitOfWork
    {
        ITransaction BeginTransaction();
    }
}
