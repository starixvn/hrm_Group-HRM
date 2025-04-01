using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Models;

namespace HRM.Services.Interfaces;

public interface IExtensionNumberService
{
    Task<IEnumerable<ExtensionNoModel>> GetAllExtensionNoAsync(short companyId);

    Task<IEnumerable<ExtensionNoModel>> GetAllExtensionGroupInfoAsync(short companyId, int? teamId = null, bool? isInternal = null);

    Task<ExtensionNoModel> GetEmployeeExtensionNoByIdAsync(int employeeExtensionNoId, bool isInternal, short companyId);

    Task<CommandResult> DeleteExtensionNumberAsync(int extensionNoId);

    Task<CommandResult> DeleteExternalGroupAsync(int teamId);
}