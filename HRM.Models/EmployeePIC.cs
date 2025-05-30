namespace HRM.Models;

public class EmployeePIC : EntityBase<int>
{
    public string NickName { get; set; }

    public string FullName { get; set; }

    public string CompanyName { get; set; }

    public string DisplayNameWithCompany => NickName + " (" + CompanyName + ")";

}