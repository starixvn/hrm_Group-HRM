using System.Collections.Generic;
using System.Text.Json;

namespace HRM.Models;

public class CompanySetting : Company
{
    public string HasBIO { get; set; }

    public string WhitelistIPs { get; set; }

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
}