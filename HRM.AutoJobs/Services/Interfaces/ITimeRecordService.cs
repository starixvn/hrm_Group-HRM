namespace HRM.AutoJobs.Services
{
    using System.Threading.Tasks;

    public interface ITimeRecordService
    {
        Task ImportTimeRecordAsync();
    }
}