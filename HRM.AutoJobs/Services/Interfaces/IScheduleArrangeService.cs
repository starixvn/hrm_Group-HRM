using System.Threading.Tasks;

namespace HRM.AutoJobs.Services.Interfaces;

public interface IScheduleArrangeService
{
    Task PublicScheduleAsync();

    Task InsertDefaultShiftAsync();

    Task InsertWeeklyDefaultShiftAsync();
}