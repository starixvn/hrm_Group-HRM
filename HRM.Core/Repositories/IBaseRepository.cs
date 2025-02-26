namespace HRM.Core.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Models;

    public interface IBaseRepository
    {
        CommandResult ExecuteNonQuery(ICriteria criteria);

        Task<CommandResult> ExecuteNonQueryAsync(ICriteria command);
    }
}