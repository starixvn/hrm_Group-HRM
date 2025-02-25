namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Overtime;

    public interface IOvertimeRequestRepository
    {
        IEnumerable<MonthlyOvertimeRequest> GetMonthlyRequestByEmployeeId(ICriteria criteria);

        MonthlyOvertimeRequest GetMonthlyRequestDetailsByEmployeeId(ICriteria criteria);

        IEnumerable<MonthlyOvertimeRequest> GetMemberMonthlyRequest(ICriteria criteria);
    }
}