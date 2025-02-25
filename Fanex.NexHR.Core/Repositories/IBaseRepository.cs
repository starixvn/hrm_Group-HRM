namespace Fanex.NexHR.Core.Repositories
{
    using Fanex.Data.Repository;
    using Models;

    public interface IBaseRepository
    {
        CommandResult ExecuteNonQuery(ICriteria criteria);
    }
}