namespace HRM.AutoJobs.Services
{
    using System.Threading.Tasks;

    public interface ISyncWorkingShiftService
    {
        Task SyncWorkingShiftAsync();
    }
}