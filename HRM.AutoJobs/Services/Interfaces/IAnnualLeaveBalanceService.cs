namespace HRM.AutoJobs.Services;

using System.Threading.Tasks;

public interface IAnnualLeaveBalanceService
{
    Task InsertForNextYearAsync();

    Task InsertForNewEmployeeAsync();

    Task RemindUsedALAsync();
}