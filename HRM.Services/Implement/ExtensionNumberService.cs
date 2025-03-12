using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.ExtensionNumber;
using HRM.Models;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class ExtensionNumberService(IBaseService baseService) : IExtensionNumberService
{
    public async Task<IEnumerable<ExtensionNoModel>> GetAllExtensionNoAsync(short companyId)
    {
        var criteria = new GetAllExtensionNumberCriteria { CompanyId = companyId };
        return await baseService.GetDataAsync<ExtensionNoModel>(criteria);
    }

    public async Task<IEnumerable<ExtensionNoModel>> GetAllExtensionGroupInfoAsync(short companyId, int? teamId = null, bool? isInternal = null)
    {
        var criteria = new GetExtensionNumberGroupInfoCriteria
        {
            TeamId = teamId,
            IsInternal = isInternal,
            CompanyId = companyId
        };
        return await baseService.GetDataAsync<ExtensionNoModel>(criteria);
    }

    public async Task<ExtensionNoModel> GetEmployeeExtensionNoByIdAsync(int employeeExtensionNoId, bool isInternal, short companyId)
    {
        var criteria = new GetExtensionNumberMemberByIdCriteria
        {
            EmployeeExtensionNoId = employeeExtensionNoId,
            IsInternal = isInternal,
            CompanyId = companyId
            
        };
        return await baseService.GetDataEntityAsync<ExtensionNoModel>(criteria);
    }

    public async Task<CommandResult> DeleteExtensionNumberAsync(int extensionNoId)
    {
        var criteria = new DeleteExtensionNumberMemberCriteria { ExtensionNumberId = extensionNoId };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> DeleteExternalGroupAsync(int teamId)
    {
        var criteria = new DeleteExternalGroupCriteria { ExternalId = teamId };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}