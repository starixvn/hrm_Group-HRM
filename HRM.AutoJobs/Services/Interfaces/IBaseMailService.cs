using System.Threading.Tasks;
using HRM.AutoJobs.Models.MailDeliver;

namespace HRM.AutoJobs.Services.Interfaces;

public interface IBaseMailService
{
    Task<bool> SendEmailAsync(MailRecord email);
}