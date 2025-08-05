using System;

namespace HRM.Models.Group;

public class GroupEmployeeModel
{
    public string CtrId { get; set; }

    public string EmpId { get; set; }

    public string EmployeeCode { get; set; }

    public string NickName { get; set; }

    public string FullName { get; set; }

    public bool IsLocal { get; set; }

    public bool IsSpecial { get; set; }

    public DateTime? CompanyDate { get; set; }

    public DateTime? ResignedDate { get; set; }

    public string UnitId { get; set; }

    public string UnitCode { get; set; }

    public string UnitPIC { get; set; }

    public string DepartmentName { get; set; }
    
    public string DepartmentPIC { get; set; }
    
    public string TeamName { get; set; }
    
    public string TeamPIC { get; set; }

    public string TeamCode{ get; set; }

    public DateTime? JoinedDate { get; set; }

    public string JobLevel { get; set; }

    public string JobTitle { get; set; }

    public string ManagementLevel { get; set; }

    public string Company { get; set; }

    public int CompanyId { get; set; }

    public string SubCompany { get; set; }

    public bool Enabled { get; set; }

    public bool IsDeleted { get; set; }

    public string DepartmentCode { get; set; }

    public string AccountableArea { get; set; }

    public string LocalCurrency { get; set; }

    public string Location { get; set; }
}