using System.Threading.Tasks;

namespace HRM.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string receiverEmail, string mailCc, string subject, string body);
    void SendEmail(string receiverEmail, string mailCc, string subject, string body);
}