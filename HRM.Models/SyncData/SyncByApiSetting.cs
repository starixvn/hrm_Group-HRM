namespace HRM.Models.SyncData;

public class SyncByApiSetting
{
    public string BaseUrl { get; set; } = "https://owp1.oneops.net/api";

    public string SyncShiftUrl { get; set; } = "/SyncWorkingShift/2021-09-22/2021-09-22";

    public string AuthenticateUrl { get; set; } = "/Token/GetToken";

    public string AppCredentialId { get; set; } = "M8dgUChV3RoYdl4fXVA7OTRm9SaO6D9c";

    public string AppCredentialPassword { get; set; } = "l4fXVA7OTRm9SaO6D9cM8dgUChV3RoYd";
}