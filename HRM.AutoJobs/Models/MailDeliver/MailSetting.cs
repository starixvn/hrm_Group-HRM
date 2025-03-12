namespace HRM.AutoJobs.Models.MailDeliver;

public class MailSetting
{
    public int AppId { get; set; }

    public string Server { get; set; }

    public int Port { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public bool IsAuth { get; set; }
}