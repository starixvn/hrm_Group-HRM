namespace HRM.Services.Implement;

using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.CompanyEmployee;
using HRM.Repositories.Interfaces;
using Interfaces;
using Models;

public class SecondCompanyEmployeeService(
    IBaseService baseService) : ISecondCompanyEmployeeService
{
    public async Task<CommandResult> CreateAsync(int actorId, int empId, int companyId)
    {
        var criteria = new InsertCompanySecondmentEmployeeCriteria
        {
            EmpId = empId,
            CompanyId = companyId,
            CreatedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> UpdateAsync(int actorId, int oldEmpId, int empId, int companyId)
    {
        var criteria = new UpdateCompanySecondmentEmployeeCriteria
        {
            EmpId = empId,
            CompanyId = companyId,
            OldEmpId = oldEmpId,
            ModifiedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> DeleteAsync(int empId, int actorId)
    {
        var criteria = new DeleteCompanySecondmentEmployeeCriteria
        {
            EmpId = empId,
            ModifiedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CompanySecondmentEmployee> GetByIdAsync(int empId)
    {
        var criteria = new GetCompanySecondmentEmployeeCriteriaByEmpId
        {
            EmpId = empId
        };
        return await baseService.GetDataEntityAsync<CompanySecondmentEmployee>(criteria);
    }

    public async Task<IEnumerable<CompanySecondmentEmployee>> GetAllAsync()
    {
        var criteria = new GetCompanySecondmentEmployeeCriteria{};
        var employees = await baseService.GetDataAsync<CompanySecondmentEmployee>(criteria);
        return employees ?? [];
    }
}