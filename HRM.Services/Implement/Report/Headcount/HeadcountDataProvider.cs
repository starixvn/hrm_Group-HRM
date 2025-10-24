using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Services;
using HRM.Criteria.Report.Headcount;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Interfaces.Report.Headcount;

namespace HRM.Services.Implement.Report.Headcount;

public class HeadcountDataProvider(IBaseService baseService) : IHeadcountDataProvider
{
    public async Task<HeadcountData> GetHeadcountDataAsync(int year)
    {
        var employeeInfos = await GetEmployeesInfos(year);
        var allEmployeeWFHs = await GetEmployeesWFH(year);
        var wfhEmployeeIds = new HashSet<int>(allEmployeeWFHs.Select(wfh => wfh.EmpId));

        return new HeadcountData
        {
            Year = year,
            EmployeeInfos = employeeInfos,
            AllEmployeeWFHs = allEmployeeWFHs,
            WfhEmployeeIds = wfhEmployeeIds
        };
    }

    private async Task<List<EmployeeInfo>> GetEmployeesInfos(int year)
    {
        var criteria = new GetHeadcountReportCriteria { Year = year };
        return (await baseService.GetDataAsync<EmployeeInfo>(criteria)).ToList();
    }

    private async Task<List<EmployeeWFH>> GetEmployeesWFH(int year)
    {
        var criteria = new GetEmployeeWFHCriteria { Year = year };
        return (await baseService.GetDataAsync<EmployeeWFH>(criteria)).ToList();
    }
}

