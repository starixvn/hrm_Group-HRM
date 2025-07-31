namespace HRM.Models.EmployeeInformation;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.Enums;
using Supervisor;
using GO.UserControl.Client.Core.Models.Role;

public class EmployeeFullProfile
{
    public int EmpId { get; set; }

    public string Code { get; set; }
    public string FullName { get; set; }
    public string Nickname { get; set; }

    public string ChineseName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public byte? Gender { get; set; }

    public byte Marriage { get; set; }

    public string MarriageName => ((MarriageStatus)Marriage).ToString();

    public string CurrentAddress { get; set; }

    public string Address { get; set; }

    public string CardNo { get; set; }

    public DateTime? DateOfIdCard { get; set; }

    public string PlaceOfIdCard { get; set; }

    public string PlaceOfBirth { get; set; }

    public string HomePhone { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public string Qualification { get; set; }

    public string Major { get; set; }

    public string TeamName { get; set; }
    public string DepartmentName { get; set; }
    public string UnitName { get; set; }
    public string UnitCode { get; set; }

    public string EmployeeCard { get; set; }

    public string CompanyName { get; set; }

    public string SubCompanyName { get; set; }

    [DataType(DataType.Date)]
    public DateTime? CompanyDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? ConfirmedDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? ResignedDate { get; set; }

    public string ResignedReason { get; set; }

    [DataType(DataType.Date)]
    public DateTime? MilitaryFromDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? MilitaryToDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? JoinedDate { get; set; }

    public string JobTitle { get; set; }
    public string JobLevel { get; set; }
    public string ManagementLevel { get; set; }
    [DataType(DataType.Date)]
    public DateTime? TeamDate { get; set; }

    public string Ethnic { get; set; }

    public string Native { get; set; }

    public string Religion { get; set; }

    public string Nationality { get; set; }

    public string PassportNo { get; set; }

    public DateTime? PassportExp { get; set; }

    public string PickupRemark { get; set; }

    public string LetsTalk { get; set; }

    public bool CanOT { get; set; }

    public decimal? SpecialOT { get; set; }

    public int WorkingDays { get; set; }

    public int WorkingHours { get; set; }

    public decimal OffDaysbyWeek { get; set; }

    public decimal AnnualLeaveBalance { get; set; }

    public decimal KeepInViewBalance { get; set; }

    public IList<Role> Roles { get; set; }

    public string Roles2String
    {
        get
        {
            return Roles == null ? string.Empty : string.Join(", ", Roles.Select(i => i.Name));
        }
    }

    public bool IsTopManager { get; set; }

    public bool IsLocal { get; set; }

    public IList<SupervisorNameModel> Supervisors { get; set; }
}