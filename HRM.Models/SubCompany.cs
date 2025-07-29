namespace HRM.Models;

public class SubCompany
{
    public int SubCompanyId { get; set; }

    public int CompanyId { get; set; }

    public string Name { get; set; }

    public int EmployeeCount { get; set; }

    public string ActionType { get; set; }
}