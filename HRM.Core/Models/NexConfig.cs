namespace HRM.Core.Models;

using System;
using System.Collections.Generic;

public class NexConfig
{
    public string NexHRId { get; set; }
    public string NexHRPassword { get; set; }
    public Dictionary<string, string> CompanyUrls { get; set; } = new();

    public Uri GetBaseUrlByCompanyId(short companyId) => new Uri(CompanyUrls.GetValueOrDefault(companyId.ToString(), ""));
}