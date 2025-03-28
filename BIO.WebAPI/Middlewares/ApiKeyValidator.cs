using Microsoft.Extensions.Configuration;

namespace BIO.WebAPI.Middlewares;

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

        var apiKey = configuration.GetValue<string>("ApiKey");
        return !string.IsNullOrWhiteSpace(apiKey) && apiKey == userApiKey;
    }
}