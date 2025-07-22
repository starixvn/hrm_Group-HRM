using System.Threading.Tasks;
using HRM.Models.LetsTalk;

namespace HRM.Services.Interfaces;

public interface ILetstalkService
{
    Task SendNotificationAsync(string letstalkId, string content, ReplyMarkup replyMarkup = null);
    void SendNotification(string letstalkId, string content, ReplyMarkup replyMarkup = null);
}