namespace Fanex.NexHR.MailDeliver
{
    public interface IBaseMailService
    {
        bool SendEmailAsync(MailRecord email);
    }
}