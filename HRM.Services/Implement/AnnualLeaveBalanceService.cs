namespace HRM.Services
{
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.AnnualLeave;
    using HRM.Models.AnnualLeaveBalance;

    public class AnnualLeaveBalanceService(IBaseService baseService) : IAnnualLeaveBalanceService
    {
        public CommandResult Adjust(int actorId, AdjustAnnualLeaveBalance model)
        {
            var criteria = new AdjustEmployeeAnnualLeaveCriteria
            {
                EmployeeIds = model.EmployeeIds,
                Description = model.Description,
                CreditDays = model.CreditDays,
                DeductDays = model.DeductDays,
                FromDate = model.AffectedFrom,
                ActorId = actorId
            };

            return baseService.ExecuteNonQuery(criteria);
        }
    }
}