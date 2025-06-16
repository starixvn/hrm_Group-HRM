using System;
using HRM.Core.Constants;

namespace HRM.Models.Report.HeadcountReport;

public class EmployeeInfo
{
    public int UserId { get; set; }
    public int EmpId { get; set; }
    public string Unit { get; set; }
    public string UnitCategory { get; set; }
    public string Nickname { get; set; }
    public string Company { get; set; }
    public string Country { get; set; }
    public string LocationCode { get; set; }
    public DateTime? JoinedDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public DateTime? ResignedDate { get; set; }
    public bool IsLocal { get; set; }
    public UserType UserType => IsLocal ? UserType.Local : UserType.Expat;
}
