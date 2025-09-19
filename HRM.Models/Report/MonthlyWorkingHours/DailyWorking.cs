namespace HRM.Models.Report.MonthlyWorkingHours;

using System;
using Core.Constants;

public class DailyWorking
{
    public int EmpId { get; set; }
    public DateTime WorkDate { get; set; }
    public float TotalWorkingHours { get; set; }
    public bool IsMissingCheckIn { get; set; }
    public bool IsMissingCheckOut { get; set; }
    public bool IsAbsent { get; set; }
    public bool IsHoliday { get; set; }
    public bool IsOffDay { get; set; }
    public bool IsLeaveDay { get; set; }
    public decimal LeaveDay { get; set; }

    public int LeaveTypeId { get; set; }
    public string LeaveTypeCode { get; set; }
    public string LeaveTypeName { get; set; }

    public int? LeaveTypeId2 { get; set; }
    public string LeaveTypeCode2 { get; set; }
    public string LeaveTypeName2 { get; set; }

    public byte RequestStatusId { get; set; }

    public bool IsLeaveHalfDay => IsLeaveDay && LeaveDay == Constants.HalfDayOffValue;

    public string TotalWorkingHoursToDisplay
    {
        get
        {
            const int PadLeft = 2;
            var hour = (int)TotalWorkingHours;
            var minute = (int)Math.Round((TotalWorkingHours - hour) * 60);

            return hour > 0
                ? hour.ToString().PadLeft(PadLeft, '0') + "h" + minute.ToString().PadLeft(PadLeft, '0')
                : minute.ToString().PadLeft(PadLeft, '0');
        }
    }
}