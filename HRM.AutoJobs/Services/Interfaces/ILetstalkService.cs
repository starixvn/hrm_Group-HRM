using System.Threading.Tasks;

namespace HRM.AutoJobs.Services.Interfaces;

public interface ILetstalkService
{
    Task SendNotificationAsync(string letstalkId, string subject, string message);
}