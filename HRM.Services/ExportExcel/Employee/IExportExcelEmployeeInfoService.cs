namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Models;
    using HRM.Models.EmployeeInformation;

    public interface IExportExcelEmployeeInfoService
    {
        Task ExportEmployeeBasicInfo(IList<Employee> employees);

        void ExportEmployeeDetailsInfo(IList<Employee> employees);

        Task ExportEmployeeFullProfile(IList<EmployeeFullProfile> profiles);
    }
}