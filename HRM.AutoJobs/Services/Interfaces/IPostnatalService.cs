namespace HRM.AutoJobs.Services
{
    using System.Threading.Tasks;

    public interface IPostnatalService
    {
        Task UpdateWorkingHourPerDayAsync();
    }
}