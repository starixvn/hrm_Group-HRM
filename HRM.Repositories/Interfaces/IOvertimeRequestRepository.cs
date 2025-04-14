namespace HRM.Repositories.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Models.Overtime;

public interface IOvertimeRequestRepository
{
    Task<IEnumerable<MonthlyOvertimeRequest>> GetMonthlyRequestByEmployeeId(ICriteria criteria);

    Task<MonthlyOvertimeRequest> GetMonthlyRequestDetailsByEmployeeId(ICriteria criteria);

    Task<IEnumerable<MonthlyOvertimeRequest>> GetMemberMonthlyRequestAsync(ICriteria criteria);
}