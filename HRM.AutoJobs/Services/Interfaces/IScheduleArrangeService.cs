namespace HRM.AutoJobs.Services;

using System.Threading.Tasks;

public interface IScheduleArrangeService
{
    Task PublicScheduleAsync();

    Task InsertDefaultShiftAsync();

    Task InsertWeeklyDefaultShiftAsync();
}