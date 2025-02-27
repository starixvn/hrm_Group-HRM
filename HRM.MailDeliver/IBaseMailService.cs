using System.Threading.Tasks;

namespace HRM.MailDeliver
{
    public interface IBaseMailService
    {
        Task<bool> SendEmailAsync(MailRecord email);
        bool SendEmail(MailRecord email);
    }
}