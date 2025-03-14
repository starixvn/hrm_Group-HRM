namespace HRM.MailDeliver;

public class MailStatus
{
    public bool IsSuccessful { get; set; }

    public string ErrorMessage { get; set; }

    public string StackTrace { get; set; }
}