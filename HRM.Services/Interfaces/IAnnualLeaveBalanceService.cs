namespace HRM.Services;

using System.Threading.Tasks;
using HRM.Core.Models;
using Models.AnnualLeaveBalance;

public interface IAnnualLeaveBalanceService
{
    Task<CommandResult> AdjustAsync(int actorId, AdjustAnnualLeaveBalance model);
}