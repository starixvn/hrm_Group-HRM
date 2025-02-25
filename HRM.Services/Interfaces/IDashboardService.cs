namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using HRM.Models.Dashboard;

    public interface IDashboardService
    {
        IEnumerable<SummaryRequest> GetMySpace(int employeeId, DateTime date);

        IEnumerable<MemberRequest> GetMemberRequest(int employeeId, DateTime date);
    }
}