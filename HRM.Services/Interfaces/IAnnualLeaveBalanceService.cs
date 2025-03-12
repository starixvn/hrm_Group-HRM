using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Models.AnnualLeaveBalance;

namespace HRM.Services.Interfaces;

public interface IAnnualLeaveBalanceService
{
    Task<CommandResult> AdjustAsync(int actorId, AdjustAnnualLeaveBalance model);
}