using System.Threading.Tasks;
using HRM.Core;
using HRM.Core.Models;
using HRM.MailDeliver;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class EmailService(AppSettings appSettings, IBaseMailService mailService) : IEmailService
{
    public async Task SendEmailAsync(string receiverEmail, string mailCc, string subject, string body)
    {
        if (!Utilities.IsValidEmail(receiverEmail) || !appSettings.EnableSendMail)
        {
            return;
        }

        var mailItem = new MailRecord
        {
            MailTo = receiverEmail,
            MailCc = mailCc,
            Subject = subject,
            Body = body
        };

        await mailService.SendEmailAsync(mailItem);
    }
}