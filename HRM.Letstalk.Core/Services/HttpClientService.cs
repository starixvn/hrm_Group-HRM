using System.Diagnostics.CodeAnalysis;
using HRM.Letstalk.Core.Services.Interfaces;

namespace HRM.Letstalk.Core.Services;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

#pragma warning disable S4462 // Calls to "async" methods should not be blocking
[SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
public class HttpClientService : IHttpClientService
{
    private static readonly HttpClient httpClient = new();

    public T ExecuteGet<T>(string baseUrl, string token = null, IDictionary<string, object> data = null) where T : new()
        => SendHttpClient<T>(baseUrl, token, HttpMethod.Get, data);

    public T ExecutePost<T>(string baseUrl, string token, IDictionary<string, object> data) where T : new()
        => SendHttpClient<T>(baseUrl, token, HttpMethod.Post, data);

    public Task<T> ExecuteGetAsync<T>(string baseUrl, string token = null, IDictionary<string, object> data = null) where T : new()
        => SendHttpClientAsync<T>(baseUrl, token, HttpMethod.Get, data);

    public Task<T> ExecutePostAsync<T>(string baseUrl, string token, IDictionary<string, object> data) where T : new()
        => SendHttpClientAsync<T>(baseUrl, token, HttpMethod.Post, data);

    private static async Task<T> SendHttpClientAsync<T>(string baseUrl, string token, HttpMethod method, IDictionary<string, object> data)
        where T : new()
    {
        ValidateEndPoint(baseUrl);
        using var request = CreateRequest(baseUrl, method, token);
        if (data != null)
        {
            var encodedData = JsonConvert.SerializeObject(data);
            var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
            request.Content = content;
        }

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
        ThrowExceptionWhenNotSuccess(response);
        if (response.IsSuccessStatusCode)
        {
            var responseData = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(responseData);

            return result;
        }

        return default;
    }

    private static T SendHttpClient<T>(string baseUrl, string token, HttpMethod method, IDictionary<string, object> data)
        where T : new()
    {
        ValidateEndPoint(baseUrl);
        using var request = CreateRequest(baseUrl, method, token);
        if (data != null)
        {
            var encodedData = JsonConvert.SerializeObject(data);
            var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
            request.Content = content;
        }

        using var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result;
        ThrowExceptionWhenNotSuccess(response);
        if (response.IsSuccessStatusCode)
        {
            var responseData = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(responseData);

            return result;
        }

        return default;
    }

    private static HttpRequestMessage CreateRequest(string baseUrl, HttpMethod method, string token)
    {
        var url = new Uri(baseUrl);
        var request = new HttpRequestMessage(method, url);
        request.Headers.Accept.Clear();
        if (token != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return request;
    }

    private static void ValidateEndPoint(string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentNullException(nameof(baseUrl));
        }
    }

    private static void ThrowExceptionWhenNotSuccess(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var message = response.Content.ReadAsStringAsync().Result;
            var exception = new Exception(message);
            throw exception;
        }
    }
}