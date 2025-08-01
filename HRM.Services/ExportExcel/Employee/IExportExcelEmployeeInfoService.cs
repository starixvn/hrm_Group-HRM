using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Models.EmployeeInformation;
using HRM.Models.Group;

namespace HRM.Services.ExportExcel.Employee;

public interface IExportExcelEmployeeInfoService
{
    Task<byte[]> ExportEmployeeBasicInfo(IList<Models.Employee> employees);

    Task<byte[]> ExportEmployeeFullProfile(IEnumerable<EmployeeFullProfile> profiles);

    Task<byte[]> ExportGroupEmployeesAsync(IEnumerable<GroupEmployeeModel> employees);

    Task<byte[]> ExportProfitCenterReportAsync(IEnumerable<GroupEmployeeModel> employees);
}