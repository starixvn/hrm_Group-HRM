namespace Fanex.NexHR.Services
{
    public interface IEmailService
    {
        void SendEmail(string senderEmail, string receiverEmail, string mailCC, string subject, string body);

        void SendEmail(string senderEmail, string senderName, string receiverEmail, string mailCC, string subject, string body);
    }
}