using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRM.Letstalk.Core.Services.Interfaces;

public interface IHttpClientService
{
    T ExecuteGet<T>(string baseUrl, string token = null, IDictionary<string, object> data = null) where T : new();

    T ExecutePost<T>(string baseUrl, string token, IDictionary<string, object> data) where T : new();

    Task<T> ExecuteGetAsync<T>(string baseUrl, string token = null, IDictionary<string, object> data = null) where T : new();

    Task<T> ExecutePostAsync<T>(string baseUrl, string token, IDictionary<string, object> data) where T : new();
}