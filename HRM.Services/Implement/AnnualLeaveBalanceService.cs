using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.AnnualLeave;
using HRM.Models.AnnualLeaveBalance;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class AnnualLeaveBalanceService(IBaseService baseService) : IAnnualLeaveBalanceService
{
    public async Task<CommandResult> AdjustAsync(int actorId, AdjustAnnualLeaveBalance model)
    {
        var criteria = new AdjustEmployeeAnnualLeaveCriteria
        {
            CompanyId = model.CompanyId,
            EmployeeIds = model.EmployeeIds,
            Description = model.Description,
            CreditDays = model.CreditDays,
            DeductDays = model.DeductDays,
            FromDate = model.AffectedFrom,
            ActorId = actorId
        };

        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}