namespace HRM.AutoJobs.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces;
using HRM.Letstalk.Core.Services.Interfaces;
using Models;

public class LetstalkService(
    LetstalkSettings letstalkSettings,
    ISendNotificationService sendNotificationService)
    : ILetstalkService
{
	private const string TemplateContent = "<strong>{0}</strong><br /> {1}";

	public async Task SendNotificationAsync(string letstalkId, string subject, string message)
    {
        var letstalk = !string.IsNullOrWhiteSpace(letstalkSettings.TestingAccountId) ? letstalkSettings.TestingAccountId : letstalkId;
        if (letstalkSettings.Enable && !string.IsNullOrWhiteSpace(letstalk))
        {
            var jwtToken = await sendNotificationService.GetTokenAsync(letstalkSettings.BaseUrl, letstalkSettings.Username, letstalkSettings.Password);
            if (jwtToken != null)
            {
                var content = string.Format(TemplateContent, subject, message);
                var data = new Dictionary<string, object>
                {
                    { "BotId", letstalkSettings.BotId },
                    { "ConversationId", letstalk },
                    { "Message", content }
                };

                await sendNotificationService.SendNotificationToLetstalkAsync(letstalkSettings.BaseUrl, jwtToken.Token, data);
            }
        }
    }
}