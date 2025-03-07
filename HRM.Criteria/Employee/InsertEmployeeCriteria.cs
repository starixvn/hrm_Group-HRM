namespace HRM.Criteria.Employee;

using Fanex.Data.Repository;

public class InsertEmployeeCriteria : CriteriaBase
{
    public short CompanyId { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string Nickname { get; set; }
    public string Code { get; set; }
    public int TeamId { get; set; }
    public string Password { get; set; }
    public string DomainUsername { get; set; }
    public int CreateBy { get; set; }

    public override string GetSettingKey() => "Employee_Information_Insert";

    public override bool IsValid() => CompanyId > 0 && UserId > 0;
}