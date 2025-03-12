using System.Threading.Tasks;

namespace HRM.AutoJobs.Services.Interfaces;

public interface IAnnualLeaveBalanceService
{
    Task InsertForNextYearAsync();

    Task InsertForNewEmployeeAsync();

    Task RemindUsedALAsync();
}