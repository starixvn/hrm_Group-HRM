namespace HRM.Core.Models;

using System.Collections.Generic;

public class NexConfig
{
    public string NexHRId { get; set; }
    public string NexHRPassword { get; set; }
    public Dictionary<string, string> CompanyUrls { get; set; } = new();

    public string GetBaseUrlByCompanyId(short companyId) => CompanyUrls.GetValueOrDefault(companyId.ToString(), "");
}