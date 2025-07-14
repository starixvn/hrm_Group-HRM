namespace HRM.ViewModels.Company;

using System.ComponentModel.DataAnnotations;

public class EditCompanySettingViewModel
{
    [Required]
    public int CompanyId { get; set; }

    public string Name { get; set; }

    public string HasBIO { get; set; }

    public string WhitelistIPs { get; set; }

    public string Currency { get; set; }

    public string EnableLetstalk { get; set; }
}