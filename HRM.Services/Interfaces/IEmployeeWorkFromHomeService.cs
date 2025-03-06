using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Criteria.EmployeeWorkFromHome;
using HRM.Models;

namespace HRM.Services.Interfaces;

public interface IEmployeeWorkFromHomeService
{
    Task<EmployeeWorkFromHome> GetByIdAsync(int id);

    Task<CommandResult> CreateAsync(InsertEmployeeWorkFromHomeCriteria criteria);

    Task<CommandResult> UpdateAsync(UpdateEmployeeWorkFromHomeCriteria criteria);

    Task<CommandResult> DeleteAsync(int id);
}