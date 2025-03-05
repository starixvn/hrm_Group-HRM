namespace HRM.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Services;
using HRM.Criteria.Dashboard;
using HRM.Models.Dashboard;

public class DashboardService(IBaseService baseService) : IDashboardService
{
    public async Task<IEnumerable<SummaryRequest>> GetMySpaceAsync(int employeeId, DateTime date)
    {
        var criteria = new GetMySpaceCriteria
        {
            EmployeeId = employeeId,
            Date = date
        };
        return await baseService.GetDataAsync<SummaryRequest>(criteria);
    }

    public async Task<IEnumerable<MemberRequest>> GetMemberRequestAsync(int employeeId, DateTime date)
    {
        var criteria = new GetMemberRequestCriteria
        {
            EmployeeId = employeeId,
            Date = date
        };
        return await baseService.GetDataAsync<MemberRequest>(criteria);
    }
}