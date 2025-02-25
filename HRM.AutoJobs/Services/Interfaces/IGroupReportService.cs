namespace HRM.AutoJobs.Services
{
    using System.Threading.Tasks;

    public interface IGroupReportService
    {
        Task SyncEmployeeInfoAsync();
    }
}