using System;
using System.Collections.Generic;
using HRM.Core.Enums;
using HRM.Resources;

namespace HRM.ViewModels.Report;

public class EmployeeAttendanceModel
{
    public string Code { get; set; }

    public int EmpId { get; set; }

    public string FullName { get; set; }

    public string NickName { get; set; }

    public string Team { get; set; }

    public string TeamCode { get; set; }

    public DateTime AttendanceDate { get; set; }

    public int? ShiftId { get; set; }

    public string ShiftCode { get; set; }

    public string ExpectedInTime { get; set; }

    public string ExpectedOutTime { get; set; }

    public DateTime? InFixed { get; set; }

    public DateTime? OutFixed { get; set; }

    public int LateIn { get; set; }

    public int EarlyOut { get; set; }

    public decimal TotalWorkHours { get; set; }

    public bool Absence { get; set; }

    public int LeaveTypeId { get; set; }

    public bool IsRightShift { get; set; }

    public string Notes { get; set; }

    public string Remark { get; set; }

    public int IsLateIn { get; set; }

    public int IsEarlyOut { get; set; }

    public int IsMissingCheckin { get; set; }

    public int IsMissingCheckOut { get; set; }

    public int IsNotWork { get; set; }

    public int IsOffDay { get; set; }

    public bool IsPublicHoliday { get; set; }

    public bool IsWarning { get; set; }

    public bool IsOvertime { get; set; }

    public decimal? OvertimeHours { get; set; }

    public byte? OvertimeStatus { get; set; }

    public string FormattedOvertimeStatus
    {
        get
        {
            if (OvertimeStatus.HasValue)
            {
                return OvertimeStatus.Value switch
                {
                    (byte)CurrentRequestStatus.Pending => Labels.Pending.ToLowerInvariant(),
                    (byte)CurrentRequestStatus.Verified => Labels.Verified.ToLowerInvariant(),
                    (byte)CurrentRequestStatus.NotVerified => Labels.NotVerified.ToLowerInvariant(),
                    (byte)CurrentRequestStatus.Approved => Labels.Approved.ToLowerInvariant(),
                    (byte)CurrentRequestStatus.Rejected => Labels.Rejected.ToLowerInvariant(),
                    (byte)CurrentRequestStatus.Processed => Labels.Processed.ToLowerInvariant(),
                    (byte)CurrentRequestStatus.Void => Labels.Void.ToLowerInvariant(),
                    _ => string.Empty
                };
            }

            return string.Empty;
        }
    }

    public bool IsOffdayOT => IsOffDay == 1 && ShiftId != null;

    public bool IsPublicHolidayOT => IsPublicHoliday && ShiftId != null;

    public bool IsWeekend =>
        AttendanceDate.DayOfWeek is DayOfWeek.Sunday or DayOfWeek.Saturday;

    public string BUCode { get; set; }

    public string DepartmentCode { get; set; }

    public List<EmployeeAttendanceRequest> RequestList { get; set; }
}