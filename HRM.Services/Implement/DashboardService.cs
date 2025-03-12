using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Services;
using Criteria.Dashboard;
using Models.Dashboard;
using GO.UserControl.Client.Core.Session;
using Core.Constants;

public class DashboardService(IBaseService baseService,
    IUserControlSession userControlSession) : IDashboardService
{
    public async Task<IEnumerable<SummaryRequest>> GetMySpaceAsync(int employeeId, DateTime date)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetMySpaceCriteria
        {
            EmployeeId = employeeId,
            Date = date,
            CompanyId = companyId
        };
        return await baseService.GetDataAsync<SummaryRequest>(criteria);
    }

    public async Task<IEnumerable<MemberRequest>> GetMemberRequestAsync(int employeeId, DateTime date)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetMemberRequestCriteria
        {
            EmployeeId = employeeId,
            Date = date,
            CompanyId = companyId
        };
        return await baseService.GetDataAsync<MemberRequest>(criteria);
    }
}