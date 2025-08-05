namespace HRM.Models;

public class CompanySecondmentEmployee
{
    public int EmpId { get; set; }

    public int SecondCompanyId { get; set; }

    public string SecondCompanyName { get; set; }

    public int CurrentCompanyId { get; set; }

    public string CurrentCompanyName { get; set; }

    public string EmpCode { get; set; }

    public string NickName { get; set; }

    public string FullName { get; set; }

    public string Location { get; set; }
}