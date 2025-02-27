namespace HRM.Services
{
    using System.Threading.Tasks;
    using HRM.Core;
    using HRM.Core.Models;
    using MailDeliver;

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

        public void SendEmail(string receiverEmail, string mailCc, string subject, string body)
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

             mailService.SendEmail(mailItem);
        }


    }
}