namespace HRM.Letstalk.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Letstalk.Core.Models;

    public interface ISendNotificationService
    {
        JwtToken GetToken(string baseUrl, string ursername, string password);

        Task<JwtToken> GetTokenAsync(string baseUrl, string ursername, string password);

        void SendNotificationToLetstalk(string baseUrl, string token, Dictionary<string, object> data);

        Task SendNotificationToLetstalkAsync(string baseUrl, string token, Dictionary<string, object> data);
    }
}