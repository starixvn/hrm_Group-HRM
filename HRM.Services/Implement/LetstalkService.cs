namespace HRM.Services.Implement
{
    using System.Collections.Generic;
    using HRM.Core.Models;
    using HRM.Letstalk.Core.Services;
    using HRM.Services.Interfaces;

    public class LetstalkService(
        LetstalkSettings letstalkSettings,
        ISendNotificationService sendNotificationService) : ILetstalkService
    {
        private readonly string templateContent = "<strong>{0}</strong><br /> {1}";

        public void SendNotification(string letstalkId, string subject, string message)
        {
            var letstalk = !string.IsNullOrWhiteSpace(letstalkSettings.TestingAccountId) ? letstalkSettings.TestingAccountId : letstalkId;
            if (letstalkSettings.Enable && !string.IsNullOrWhiteSpace(letstalk))
            {
                var jwtToken = sendNotificationService.GetToken(letstalkSettings.BaseUrl, letstalkSettings.Username, letstalkSettings.Password);
                if (jwtToken != null)
                {
                    var content = string.Format(templateContent, subject, message);
                    var data = new Dictionary<string, object>
                    {
                        { "BotId", letstalkSettings.BotId },
                        { "ConversationId", letstalk },
                        { "Message", content }
                    };

                    sendNotificationService.SendNotificationToLetstalk(letstalkSettings.BaseUrl, jwtToken.Token, data);
                }
            }
        }
    }
}