namespace HRM.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Services;
using HRM.Core.Models;
using HRM.Criteria.ExtensionNumber;
using HRM.Models;

public class ExtensionNumberService(IBaseService baseService) : IExtensionNumberService
{
    public async Task<IEnumerable<ExtensionNoModel>> GetAllExtensionNoAsync()
    {
        var criteria = new GetAllExtensionNumberCriteria();
        return await baseService.GetDataAsync<ExtensionNoModel>(criteria);
    }

    public async Task<IEnumerable<ExtensionNoModel>> GetAllExtensionGroupInfoAsync(int? teamId = null, bool? isInternal = null)
    {
        var criteria = new GetExtensionNumberGroupInfoCriteria
        {
            TeamId = teamId,
            IsInternal = isInternal
        };
        return await baseService.GetDataAsync<ExtensionNoModel>(criteria);
    }

    public async Task<ExtensionNoModel> GetEmployeeExtensionNoByIdAsync(int employeeExtensionNoId, bool isInternal)
    {
        var criteria = new GetExtensionNumberMemberByIdCriteria
        {
            EmployeeExtensionNoId = employeeExtensionNoId,
            IsInternal = isInternal
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