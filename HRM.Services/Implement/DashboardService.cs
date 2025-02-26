namespace HRM.Services
{
    using HRM.Core.Services;
    using HRM.Criteria.Dashboard;
    using HRM.Models.Dashboard;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class DashboardService : IDashboardService
    {
        private readonly IBaseService baseService;

        public DashboardService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IEnumerable<SummaryRequest> GetMySpace(int employeeId, DateTime date)
        {
            var criteria = new GetMySpaceCriteria
            {
                EmployeeId = employeeId,
                Date = date
            };
            return baseService.GetData<SummaryRequest>(criteria);
        }

        public async Task<IEnumerable<MemberRequest>> GetMemberRequestAsync(int employeeId, DateTime date)
        {
            var criteria = new GetMemberRequestCriteria
            {
                EmployeeId = employeeId,
                Date = date
            };
            return baseService.GetData<MemberRequest>(criteria);
        }
    }
}