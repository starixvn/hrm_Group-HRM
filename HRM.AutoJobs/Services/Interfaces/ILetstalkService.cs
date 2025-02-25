namespace HRM.AutoJobs.Services
{
    using System.Threading.Tasks;

    public interface ILetstalkService
    {
        Task SendNotificationAsync(string letstalkId, string subject, string message);
    }
}