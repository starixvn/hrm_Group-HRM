namespace HRM.Services
{
    using HRM.Core.Models;
    using HRM.Models.AnnualLeaveBalance;

    public interface IAnnualLeaveBalanceService
    {
        CommandResult Adjust(int actorId, AdjustAnnualLeaveBalance model);
    }
}