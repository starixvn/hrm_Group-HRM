namespace HRM.Core.Models;

public class AppSettings
{
    public short AppMailId { get; set; }
    public string Domain { get; set; }
    public bool EnableSendMail { get; set; }
    public bool IsShowCloseBalance { get; set; }
    public string EmailSD { get; set; }
    public string EmailGo { get; set; }
    public string WhitelistIPs { get; set; }
    public short ServerTimeZone { get; set; }
}