using System;
using System.Collections.Generic;
using System.Linq;

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
            return string.IsNullOrWhiteSpace(WhitelistIPs)
                ? new List<string>()
                : WhitelistIPs
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();
        }
    }
}