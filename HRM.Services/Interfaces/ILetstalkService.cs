using System.Threading.Tasks;

namespace HRM.Services.Interfaces;

public interface ILetstalkService
{
    Task SendNotificationAsync(string letstalkId, string subject, string message);
    void SendNotification(string letstalkId, string subject, string message);
}