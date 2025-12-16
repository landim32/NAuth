namespace NAuth.Infra.Interfaces
{
    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
