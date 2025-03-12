using HRM.AutoJobs.Services.Interfaces;

namespace HRM.AutoJobs.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Fanex.Logging;
using Criteria.GroupReport;
using Models;
using Jose;
using Newtonsoft.Json;

public class GroupReportService(
    IHttpClientFactory clientFactory,
    ILogger logger,
    IBaseService baseService,
    UserControlSettings userControlSettings) : IGroupReportService
{
    private DateTime? lastModifiedDate = null;

    public async Task SyncEmployeeInfoAsync()
    {
        var criteria = new GetEmployeeInfoCriteria
        {
            LastModifiedDate = lastModifiedDate
        };
        var employeeInfos = await baseService.GetDataAsync<EmployeeInfo>(criteria);
        if (employeeInfos != null && employeeInfos.Any())
        {
            var httpClient = clientFactory.CreateClient();
            var siteIdentity = GenerateSiteIdentity();
            httpClient.DefaultRequestHeaders.Add("SiteIdentity", $"Bearer {siteIdentity}");

            var data = new Dictionary<string, object>
            {
                { "companyId", userControlSettings.CompanyId },
                { "employeeInfos", employeeInfos }
            };
            var encodedData = JsonConvert.SerializeObject(data);
            var content = new StringContent(encodedData, Encoding.UTF8, "application/json");

            var url = new Uri($"{userControlSettings.ServiceUrl}api/EmployeeInfo/UpdateInfo");
            var response = await httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                lastModifiedDate = employeeInfos.Max(x => x.ModifiedDate);
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                await logger.ErrorAsync($"StatusCode: {response.StatusCode}\r\nMessage: {message}");
            }
        }
    }

    private string GenerateSiteIdentity()
    {
        var data = new Dictionary<string, object>
        {
            { "SiteId", userControlSettings.SiteId },
            { "AccessToken", userControlSettings.AccessToken }
        };
        return JWT.Encode(data, Encoding.UTF8.GetBytes(userControlSettings.AccessToken), JwsAlgorithm.HS256);
    }
}