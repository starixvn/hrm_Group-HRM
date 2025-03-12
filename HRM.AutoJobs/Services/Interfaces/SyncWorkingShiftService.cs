using System.Threading.Tasks;

namespace HRM.AutoJobs.Services.Interfaces;

public interface ISyncWorkingShiftService
{
    Task SyncWorkingShiftAsync();
}