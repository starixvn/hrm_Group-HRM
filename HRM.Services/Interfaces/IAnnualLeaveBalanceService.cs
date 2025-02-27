namespace HRM.Services
{
    using System.Threading.Tasks;
    using HRM.Core.Models;
    using HRM.Models.AnnualLeaveBalance;

    public interface IAnnualLeaveBalanceService
    {
        Task<CommandResult> AdjustAsync(int actorId, AdjustAnnualLeaveBalance model);
    }
}