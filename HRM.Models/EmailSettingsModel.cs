namespace HRM.Models;

public class EmailSettingsModel
{
    public int TeamId { get; set; }

    public int CompanyId { get; set; }

    public int Action { get; set; }

    public bool AllowSendUser { get; set; }

    public bool AllowSendVerifier { get; set; }

    public bool AllowSendApprover { get; set; }

    public bool AllowSendHR { get; set; }

    public bool AllowSendMailGroup { get; set; }
}