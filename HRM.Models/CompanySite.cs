namespace HRM.Models;

public class CompanySite
{
    public short CompanyId { get; set; }
    public short SiteId { get; set; }
    public string AccessToken { get; set; }
    public string Domain { get; set; }
}