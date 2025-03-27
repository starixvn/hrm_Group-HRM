using Microsoft.Extensions.Configuration;

namespace HRM.Controllers.API.Middlewares;

public interface IApiKeyValidator
{
    bool IsValid(string userApiKey);
}

public class ApiKeyValidator(IConfiguration configuration) : IApiKeyValidator
{
    public bool IsValid(string userApiKey)
    {
        if (string.IsNullOrWhiteSpace(userApiKey))
        {
            return false;
        }

        var apiKey = configuration.GetValue<string>("FnBSyncApiKey");
        return !string.IsNullOrWhiteSpace(apiKey) && apiKey == userApiKey;
    }
}