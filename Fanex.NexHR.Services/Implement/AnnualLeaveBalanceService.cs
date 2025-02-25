namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Models.AnnualLeaveBalance;

    public class AnnualLeaveBalanceService : IAnnualLeaveBalanceService
    {
        private readonly IBaseService baseService;

        public AnnualLeaveBalanceService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

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