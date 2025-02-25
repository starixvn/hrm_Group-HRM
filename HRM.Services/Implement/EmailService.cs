namespace HRM.Services
{
    using HRM.Core;
    using MailDeliver;

    public class EmailService : IEmailService
    {
        private readonly IBaseMailService mailService;

        public EmailService(IBaseMailService mailService)
        {
            this.mailService = mailService;
        }

        public void SendEmail(string senderEmail, string receiverEmail, string mailCC, string subject, string body)
        {
            SendEmail(senderEmail, senderName: senderEmail, receiverEmail: receiverEmail, mailCC: mailCC, subject: subject, body: body);
        }

        public void SendEmail(string senderEmail, string senderName, string receiverEmail, string mailCC, string subject, string body)
        {
            if (!Utilities.IsValidEmail(senderEmail) || !Utilities.IsValidEmail(receiverEmail) || ConfigSettings.IsDisabledSendMail)
            {
                return;
            }

            var mailItem = new MailRecord
            {
                MailTo = receiverEmail,
                MailCc = mailCC,
                Subject = subject,
                Body = body
            };

            mailService.SendEmailAsync(mailItem);
        }
    }
}