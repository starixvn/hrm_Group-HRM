namespace HRM.Services
{
    public interface IEmailService
    {
        void SendEmail(string receiverEmail, string mailCc, string subject, string body);
    }
}