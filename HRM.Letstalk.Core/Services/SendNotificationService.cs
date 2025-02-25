namespace HRM.Letstalk.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Letstalk.Core.Constants;
    using HRM.Letstalk.Core.Models;

    public class SendNotificationService : ISendNotificationService
    {
        private readonly IHttpClientService httpClientService;

        public SendNotificationService(IHttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }

        public JwtToken GetToken(string baseUrl, string ursername, string password)
        {
            var url = $"{baseUrl}{LetstalkApiPath.Auth}?username={ursername}&password={password}";
            var token = httpClientService.ExecuteGet<JwtToken>(url);
            return token;
        }

        public async Task<JwtToken> GetTokenAsync(string baseUrl, string ursername, string password)
        {
            var url = $"{baseUrl}{LetstalkApiPath.Auth}?username={ursername}&password={password}";
            var token = await httpClientService.ExecuteGetAsync<JwtToken>(url);
            return token;
        }

        public void SendNotificationToLetstalk(string baseUrl, string token, Dictionary<string, object> data)
        {
            var url = baseUrl + LetstalkApiPath.Notify;
            httpClientService.ExecutePost<object>(url, token, data);
        }

        public async Task SendNotificationToLetstalkAsync(string baseUrl, string token, Dictionary<string, object> data)
        {
            var url = baseUrl + LetstalkApiPath.Notify;
            await httpClientService.ExecutePostAsync<object>(url, token, data);
        }
    }
}