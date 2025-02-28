namespace HRM.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Models;

public interface IExtensionNumberService
{
    Task<IEnumerable<ExtensionNoModel>> GetAllExtensionNoAsync();

    Task<IEnumerable<ExtensionNoModel>> GetAllExtensionGroupInfoAsync(int? teamId = null, bool? isInternal = null);

    Task<ExtensionNoModel> GetEmployeeExtensionNoByIdAsync(int employeeExtensionNoId, bool isInternal);

    Task<CommandResult> DeleteExtensionNumberAsync(int extensionNoId);

    Task<CommandResult> DeleteExternalGroupAsync(int teamId);
}