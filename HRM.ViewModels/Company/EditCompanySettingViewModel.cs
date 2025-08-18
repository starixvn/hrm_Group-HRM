namespace HRM.ViewModels.Company;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HRM.Models;

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

    public IList<SubCompany> SubCompanies { get; set; }
}