namespace HRM.AutoJobs.Models;

public class AppSettings
{
    public int AppMailId { get; set; }
    public bool EnableSendMail { get; set; }
    public string SyncWorkingShiftUrl { get; set; }
}