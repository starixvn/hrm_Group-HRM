namespace HRM.Services.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Models;

public interface ISecondCompanyEmployeeService
{
    Task<CommandResult> CreateAsync(int actorId, int empId, int companyId);
    Task<CommandResult> UpdateAsync(int actorId, int oldEmpId, int empId, int companyId);
    Task<CommandResult> DeleteAsync(int empId);
    Task<CompanySecondmentEmployee> GetByIdAsync(int empId);
    Task<IEnumerable<CompanySecondmentEmployee>> GetAllAsync();
}