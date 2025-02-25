namespace HRM.AutoJobs.Services
{
    using System.Threading.Tasks;
    using HRM.AutoJobs.Models.MailDeliver;

    public interface IBaseMailService
    {
        Task<bool> SendEmailAsync(MailRecord email);
    }
}