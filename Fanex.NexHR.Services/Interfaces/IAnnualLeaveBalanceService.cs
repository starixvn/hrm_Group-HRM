namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models.AnnualLeaveBalance;

    public interface IAnnualLeaveBalanceService
    {
        CommandResult Adjust(int actorId, AdjustAnnualLeaveBalance model);
    }
}