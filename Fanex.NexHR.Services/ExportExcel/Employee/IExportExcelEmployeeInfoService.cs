namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.EmployeeInformation;

    public interface IExportExcelEmployeeInfoService
    {
        void ExportEmployeeBasicInfo(IList<Employee> employees);

        void ExportEmployeeDetailsInfo(IList<Employee> employees);

        void ExportEmployeeFullProfile(IList<EmployeeFullProfile> profiles);
    }
}