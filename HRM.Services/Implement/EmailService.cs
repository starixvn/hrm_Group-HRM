namespace HRM.Services
{
    using HRM.Core;
    using HRM.Core.Models;
    using MailDeliver;

    public class EmailService(AppSettings appSettings, IBaseMailService mailService) : IEmailService
    {
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

            mailService.SendEmailAsync(mailItem);
        }
    }
}