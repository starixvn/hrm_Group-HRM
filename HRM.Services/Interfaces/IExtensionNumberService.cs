namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Core.Models;
    using HRM.Models;

    public interface IExtensionNumberService
    {
        IEnumerable<ExtensionNoModel> GetAllExtensionNo();

        IList<ExtensionNoModel> GetAllExtensionGroupInfo(int? teamId = null, bool? isInternal = null);

        ExtensionNoModel GetEmployeeExtensionNoById(int employeeExtensionNoId, bool isInternal);

        CommandResult DeleteExtensionNumber(int extensionNoId);

        CommandResult CreateExternalGroup(string teamName);

        CommandResult EditExternalGroup(int teamId, string teamName);

        CommandResult DeleteExternalGroup(int teamId);
    }
}