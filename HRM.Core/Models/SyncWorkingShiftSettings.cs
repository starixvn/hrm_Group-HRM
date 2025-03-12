namespace HRM.Core.Models;

public class SyncWorkingShiftSettings
{
    public bool Enable { get; set; }
    public string ApiUrl { get; set; }
    public string LocationCode { get; set; }
    public short Timezone { get; set; }
}