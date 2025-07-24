namespace HRM.Services.Implement;

using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Logging;
using HRM.Core.Models;
using HRM.Letstalk.Core.Services.Interfaces;
using HRM.Models.LetsTalk;
using Interfaces;
using Newtonsoft.Json;

public class LetstalkService(
    LetstalkSettings letstalkSettings,
    ILogger logger,
    ISendNotificationService sendNotificationService) : ILetstalkService
{

    public async Task SendNotificationAsync(string letstalkId, string content, ReplyMarkup replyMarkup = null)
    {
        if (letstalkSettings.Enable && !string.IsNullOrWhiteSpace(letstalkId))
        {
            var jwtToken = await sendNotificationService.GetTokenAsync(letstalkSettings.BaseUrl, letstalkSettings.Username, letstalkSettings.Password);
            if (jwtToken != null)
            {
                var data = new Dictionary<string, object>
                {
                    { "BotId", letstalkSettings.BotId },
                    { "ConversationId", letstalkId },
                    { "Message", content },
                    { "ReplyMarkup", JsonConvert.SerializeObject(replyMarkup) }
                };

                await logger.InfoAsync($"Send notification to Letstalk: {JsonConvert.SerializeObject(data)}");
                await sendNotificationService.SendNotificationToLetstalkAsync(letstalkSettings.BaseUrl, jwtToken.Token, data);
            }
        }
    }

    public void SendNotification(string letstalkId, string content, ReplyMarkup replyMarkup = null)
    {
        if (letstalkSettings.Enable && !string.IsNullOrWhiteSpace(letstalkId))
        {
            var jwtToken = sendNotificationService.GetToken(letstalkSettings.BaseUrl, letstalkSettings.Username, letstalkSettings.Password);
            if (jwtToken != null)
            {
                var data = new Dictionary<string, object>
                {
                    { "BotId", letstalkSettings.BotId },
                    { "ConversationId", letstalkId },
                    { "Message", content },
                    { "ReplyMarkup", JsonConvert.SerializeObject(replyMarkup) }
                };

                logger.InfoAsync($"Send notification to Letstalk: {JsonConvert.SerializeObject(data)}");
                sendNotificationService.SendNotificationToLetstalk(letstalkSettings.BaseUrl, jwtToken.Token, data);
            }
        }
    }
}