namespace HRM.MailDeliver
{
    public interface IBaseMailService
    {
        bool SendEmailAsync(MailRecord email);
    }
}