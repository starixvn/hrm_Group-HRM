using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Letstalk.Core.Models;

namespace HRM.Letstalk.Core.Services.Interfaces;

public interface ISendNotificationService
{
    JwtToken GetToken(string baseUrl, string userName, string password);

    Task<JwtToken> GetTokenAsync(string baseUrl, string userName, string password);

    void SendNotificationToLetstalk(string baseUrl, string token, Dictionary<string, object> data);

    Task SendNotificationToLetstalkAsync(string baseUrl, string token, Dictionary<string, object> data);
}