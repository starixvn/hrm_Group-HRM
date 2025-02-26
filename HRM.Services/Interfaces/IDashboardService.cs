namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Models.Dashboard;

    public interface IDashboardService
    {
        IEnumerable<SummaryRequest> GetMySpace(int employeeId, DateTime date);

        Task<IEnumerable<MemberRequest>> GetMemberRequestAsync(int employeeId, DateTime date);
    }
}