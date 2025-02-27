namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Core.Models;
    using HRM.Models;

    public interface IMemoService
    {
        Task<IEnumerable<Memo>> GetAllAsync(int year, int category);

        Task<Memo> GetByIdAsync(int id);

        Task<CommandResult> CreateAsync(Memo model);

        Task<CommandResult> EditAsync(Memo model);

        Task<CommandResult> DeleteAsync(int id);
    }
}