namespace HRM.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using Models;

public interface IExtensionNumberService
{
    Task<IEnumerable<ExtensionNoModel>> GetAllExtensionNoAsync(short companyId);

    Task<IEnumerable<ExtensionNoModel>> GetAllExtensionGroupInfoAsync(short companyId, int? teamId = null, bool? isInternal = null);

    Task<ExtensionNoModel> GetEmployeeExtensionNoByIdAsync(int employeeExtensionNoId, bool isInternal);

    Task<CommandResult> DeleteExtensionNumberAsync(int extensionNoId);

    Task<CommandResult> DeleteExternalGroupAsync(int teamId);
}