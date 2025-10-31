using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Services;
using HRM.Criteria.Report.Headcount;
using HRM.Models.Report.HeadcountReport;

namespace HRM.Repositories.HeadcountReport;

public interface IHeadcountReportRepository
{
    Task<HeadcountData> GetHeadcountDataAsync(int year);
    Task<List<HeadcountDetailModel>> GetDetailAsync(string empIds);
}

public class HeadcountReportRepository(IBaseService baseService) : IHeadcountReportRepository
{
    public async Task<HeadcountData> GetHeadcountDataAsync(int year)
    {
        var employeeInfosTask = GetEmployeesInfos(year);
        var allEmployeeWFHsTask = GetEmployeesWFH(year);
        var empTransfersTask = GetEmployeesTransfer(year);

        await Task.WhenAll(employeeInfosTask, allEmployeeWFHsTask, empTransfersTask);
        var (transferIns, transferOuts) = await empTransfersTask;

        return new HeadcountData
        {
            Year = year,
            EmployeeInfos = await employeeInfosTask,
            AllEmployeeWFHs = await allEmployeeWFHsTask,
            EmployeeTransferIns = transferIns,
            EmployeeTransferOut = transferOuts
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

    private async Task<(List<EmployeeTransferData> transferIns, List<EmployeeTransferData> transferOuts)> GetEmployeesTransfer(int year)
    {
        var criteria = new GetEmployeeTransferCriteria { Year = year };
        var (transferIns, transferOuts) = await baseService.FetchMultipleAsync<EmployeeTransferData, EmployeeTransferData>(criteria);
        return (transferIns.ToList(), transferOuts.ToList());
    }

    public async Task<List<HeadcountDetailModel>> GetDetailAsync(string empIds)
    {
        var criteria = new GetHeadcountDetailCriteria 
        { 
            EmpIds = empIds 
        };
        return (await baseService.GetDataAsync<HeadcountDetailModel>(criteria)).ToList();
    }
}

