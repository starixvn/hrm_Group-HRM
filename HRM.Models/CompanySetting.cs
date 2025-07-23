using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HRM.Models;

public class CompanySetting : Company
{
    public string HasBIO { get; set; }

    public string WhitelistIPs { get; set; }

    [JsonPropertyName("urls")]
    public string Urls { get; set; }

    public string EnableLetstalk { get; set; }

    public string Currency { get; set; }

    public List<string> WhitelistIPList
    {
        get
        {
            try
            {
                return string.IsNullOrWhiteSpace(WhitelistIPs)
                    ? []
                    : JsonSerializer.Deserialize<List<string>>(WhitelistIPs) ?? [];
            }
            catch
            {
                return [];
            }
        }
    }

    public List<string> URLList
    {
        get
        {
            try
            {
                return string.IsNullOrWhiteSpace(Urls)
                    ? []
                    : JsonSerializer.Deserialize<List<string>>(Urls) ?? [];
            }
            catch
            {
                return [];
            }
        }
    }

    public List<SubCompany> SubCompanyList { get; set; }
}