using System.Threading.Tasks;
using HRM.Models;

namespace HRM.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string receiverEmail, string mailCc, string subject, string body, MailSetting setting = null, short? companyId = null);

    Task<MailSetting> GetMailSettingAsync(MailSetting mailSetting = null, short? companyId = null);
}