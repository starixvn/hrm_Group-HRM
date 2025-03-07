namespace HRM.Models;

public class CreateEmployeeParam
{
    public short CompanyId { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string Nickname { get; set; }
    public string DomainUsername { get; set; }
    public string Code { get; set; }
    public int TeamId { get; set; }
    public int ActorId { get; set; }
}