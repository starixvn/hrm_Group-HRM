namespace HRM.Models.ScheduleArrange;

using System;
using Core.Constants;
using Core.Enums;

[Serializable]
public class DailySchedule
{
    public int EmployeeId { get; set; }
    public DateTime WorkDate { get; set; }
    public ShiftBase Shift { get; set; }
    public bool IsHoliday { get; set; }
    public bool IsOffDay { get; set; }
    public bool IsLeaveDay { get; set; }
    public bool IsLeaveOffDay { get; set; }
    public bool IsOvertime { get; set; }
    public decimal LeaveDay { get; set; }
    public bool IsKeepInView { get; set; }

    public byte? ChangeShiftTypeId { get; set; }
    public DateTime? MoveOffFromDate { get; set; }
    public DateTime? MoveOffToDate { get; set; }
    public bool IsChangeShift => ChangeShiftTypeId.HasValue;

    public bool IsOffDayReplacement { get; set; }
    public bool IsModified { get; set; }
    public bool IsFixedOffDay { get; set; }

    public LeaveRequest LeaveType { get; set; }
    public LeaveRequest LeaveType2 { get; set; }
    public bool IsLeaveHalfDay => IsLeaveDay && !IsHoliday && !IsOffDay && LeaveDay == Constants.HalfDayOffValue;

    public bool IsPublic { get; set; }

    public bool IsDisabled
    {
        get
        {
            if (IsLeaveDay && !IsLeaveHalfDay)
            {
                return true;
            }

            if (IsChangeShift
                && (ChangeShiftTypeId.Value == (byte)ChangeShiftType.ChangeShift
                    || (ChangeShiftTypeId.Value == (byte)ChangeShiftType.MoveOffDay && MoveOffFromDate.HasValue)))
            {
                return true;
            }

            return IsLeaveOffDay || IsOvertime;
        }
    }
}