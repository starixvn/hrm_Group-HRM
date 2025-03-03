namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Models;
    using HRM.Models.EmployeeInformation;

    public interface IExportExcelEmployeeInfoService
    {
        void ExportEmployeeBasicInfo(IList<Employee> employees);

        void ExportEmployeeDetailsInfo(IList<Employee> employees);

        void ExportEmployeeFullProfile(IList<EmployeeFullProfile> profiles);
    }
}