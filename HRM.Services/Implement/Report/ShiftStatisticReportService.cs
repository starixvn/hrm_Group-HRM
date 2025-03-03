namespace HRM.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Services;
using HRM.Criteria.Report;
using HRM.Models.Report;

public interface IShiftStatisticReportService
{
    Task<IEnumerable<ShiftStatistic>> GetShiftStatisticAsync(GetShiftStatisticParam param);

    Task<IEnumerable<ShiftStatisticDetail>> GetShiftStatisticDetailAsync(GetShiftStatisticDetailParam param);
}

public class ShiftStatisticReportService(IBaseService baseService) : IShiftStatisticReportService
{
    public async Task<IEnumerable<ShiftStatistic>> GetShiftStatisticAsync(GetShiftStatisticParam param)
    {
        var criteria = new GetShiftStatisticCriteria
        {
            SupervisorId = param.SupervisorId,
            FromDate = param.FromDate,
            ToDate = param.ToDate,
            TeamIds = param.TeamIds,
            IsLocal = param.IsLocal,
            IsShowWholeCompany = param.IsShowWholeCompany
        };
        return await baseService.GetDataAsync<ShiftStatistic>(criteria);
    }

    public async Task<IEnumerable<ShiftStatisticDetail>> GetShiftStatisticDetailAsync(GetShiftStatisticDetailParam param)
    {
        var criteria = new GetShiftStatisticDetailCriteria
        {
            SupervisorId = param.SupervisorId,
            EmployeeId = param.EmployeeId,
            WorkDate = param.WorkDate,
            ShiftId = param.ShiftId,
            TeamIds = param.TeamIds,
            IsLocal = param.IsLocal,
            IsShowWholeCompany = param.IsShowWholeCompany
        };
        return await baseService.GetDataAsync<ShiftStatisticDetail>(criteria);
    }
}