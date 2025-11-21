using System;
using HRM.Core.Enums;

namespace HRM.Models.Group;

public class GroupEmployeeModel
{
    public int? CtrId { get; set; }

    public string CtrIdFormatted => CtrId is not null ? CtrId.ToString().PadLeft(6, '0') : string.Empty;

    public int EmpId { get; set; }

    public string EmployeeCode { get; set; }

    public string NickName { get; set; }

    public string FullName { get; set; }

    public bool IsLocal { get; set; }

    public bool IsSpecial { get; set; }

    public DateTime? CompanyDate { get; set; }

    public DateTime? ResignedDate { get; set; }

    public string ResignedReason { get; set; }

    public string UnitId { get; set; }

    public string GroupUnitId { get; set; }

    public string UnitCode { get; set; }
    public string UnitPIC { get; set; }
    public int UnitPICEmpId { get; set; }
    public string UnitSupporterEmpId { get; set; }

    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; }
    public string DepartmentPIC { get; set; }
    public int DepartmentPICId { get; set; }
    public string DepartmentSupporterEmpId { get; set; }

    public string TeamCode { get; set; }
    public string TeamName { get; set; }
    public int TeamPICEmpId { get; set; }
    public string TeamPIC { get; set; }

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

    public string Currency { get; set; }

    public decimal Salary { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal JobLevelAddOn { get; set; }
    public decimal ManagementAllowance { get; set; }
    public decimal OverseasAllowance { get; set; }
    public decimal SkillSetAddOn { get; set; }
    public decimal Others { get; set; }

    public string CountryName { get; set; }

    public Gender Gender { get; set; }

    public DateTime DOB { get; set; }

    public DateTime? MilitaryFromDate { get; set; }

    public DateTime? MilitaryToDate { get; set; }
}