namespace HRM.Models.Report.Overtime;

using System;

public class DailyOvertimeDetails
{
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string FullName { get; set; }
    public string Nickname { get; set; }
    public string Team { get; set; }
    public string Department { get; set; }
    public DateTime WorkDate { get; set; }
    public int? ShiftId { get; set; }
    public string ShiftCode { get; set; }
    public DateTime TimeIn { get; set; }
    public DateTime TimeOut { get; set; }
    public decimal ActualWorkingHours { get; set; }
    public decimal NormalHours { get; set; }
    public decimal OffHours { get; set; }
    public decimal PublicHours { get; set; }
    public decimal SpecialHours { get; set; }
    public string Remark { get; set; }
    public byte StatusId { get; set; }
    public bool IsOffDay { get; set; }
    public bool IsHoliday { get; set; }
}