namespace TinyUrl.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
