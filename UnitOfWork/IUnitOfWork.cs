namespace StudentsDashboard.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
