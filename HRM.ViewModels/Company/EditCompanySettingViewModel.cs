namespace HRM.ViewModels.Company;

using HRM.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class EditCompanySettingViewModel
{
    [Required]
    public int CompanyId { get; set; }

    public string Name { get; set; }

    public string HasBIO { get; set; }

    public string WhitelistIPs { get; set; }

    public string Currency { get; set; }

    public string EnableLetstalk { get; set; }

    public string SettingsJson { get; set; }

    public List<SubCompany> SubCompanies { get; set; }
}