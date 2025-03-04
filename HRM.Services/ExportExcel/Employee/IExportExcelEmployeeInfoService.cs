using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Models.EmployeeInformation;

namespace HRM.Services.ExportExcel.Employee
{
    public interface IExportExcelEmployeeInfoService
    {
        Task ExportEmployeeBasicInfo(IList<Models.Employee> employees);

        void ExportEmployeeDetailsInfo(IList<Models.Employee> employees);

        Task ExportEmployeeFullProfile(IList<EmployeeFullProfile> profiles);
    }
}