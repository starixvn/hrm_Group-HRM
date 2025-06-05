using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Services;
using HRM.Criteria.SystemSetting;
using HRM.MailDeliver;
using HRM.Models;
using HRM.Services.Interfaces;
using Newtonsoft.Json;

namespace HRM.Services.Implement;

public class EmailService(IBaseMailService mailService,IBaseService baseService, IUserControlSession session) : IEmailService
{
    public async Task SendEmailAsync(string receiverEmail, string mailCc, string subject, string body, MailSetting setting = null, short? companyId = null)
    {
        var mailSetting = await GetMailSettingAsync(setting, companyId);
        if (!Utilities.IsValidEmail(receiverEmail) || !mailSetting.Enable)
        {
            return;
        }

        var mailItem = new MailRecord
        {
            MailTo = receiverEmail,
            MailCc = mailCc,
            Subject = subject,
            Body = body,
            AppMailId = mailSetting.AppMailId
        };

        await mailService.SendEmailAsync(mailItem);
    }

    public async Task<MailSetting> GetMailSettingAsync(MailSetting mailSetting = null, short? companyId = null)
    {
        companyId ??= session.Get<short?>(SessionKey.CurrentCompanyId);
        var criteria = new GetSystemConfigsByKeyCriteria
        {
            CompanyId = companyId.Value,
            Key = SystemConfigKey.MailSetting
        };

        var data = await baseService.GetDataEntityAsync<string>(criteria);
        var result = data == null ? null : JsonConvert.DeserializeObject<MailSetting>(data);
        return result;
    }
}