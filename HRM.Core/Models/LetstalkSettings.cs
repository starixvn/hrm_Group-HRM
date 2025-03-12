namespace HRM.Core.Models;

public class LetstalkSettings
{
    public bool Enable { get; set; }
    public int BotId { get; set; }
    public string TestingAccountId { get; set; }
    public string BaseUrl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}