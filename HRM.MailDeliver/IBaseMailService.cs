namespace HRM.MailDeliver;

using System.Threading.Tasks;

public interface IBaseMailService
{
    Task<bool> SendEmailAsync(MailRecord email);
}