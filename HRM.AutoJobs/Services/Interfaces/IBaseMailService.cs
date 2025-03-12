namespace HRM.AutoJobs.Services;

using System.Threading.Tasks;
using Models.MailDeliver;

public interface IBaseMailService
{
    Task<bool> SendEmailAsync(MailRecord email);
}