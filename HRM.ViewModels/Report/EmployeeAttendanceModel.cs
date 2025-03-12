namespace HRM.ViewModels;

using System;
using System.Collections.Generic;
using Core.Enums;
using Resources;

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
                switch (OvertimeStatus.Value)
                {
                    case (byte)CurrentRequestStatus.Pending:
                        return Labels.Pending.ToLowerInvariant();

                    case (byte)CurrentRequestStatus.Verified:
                        return Labels.Verified.ToLowerInvariant();

                    case (byte)CurrentRequestStatus.NotVerified:
                        return Labels.NotVerified.ToLowerInvariant();

                    case (byte)CurrentRequestStatus.Approved:
                        return Labels.Approved.ToLowerInvariant();

                    case (byte)CurrentRequestStatus.Rejected:
                        return Labels.Rejected.ToLowerInvariant();

                    case (byte)CurrentRequestStatus.Processed:
                        return Labels.Processed.ToLowerInvariant();

                    case (byte)CurrentRequestStatus.Void:
                        return Labels.Void.ToLowerInvariant();

                    default:
                        return string.Empty;
                }
            }

            return string.Empty;
        }
    }

    public bool IsOffdayOT
    {
        get
        {
            return IsOffDay == 1 && ShiftId != null;
        }
    }

    public bool IsPublicHolidayOT
    {
        get
        {
            return IsPublicHoliday && ShiftId != null;
        }
    }

    public bool IsWeekend
    {
        get
        {
            return AttendanceDate.DayOfWeek == DayOfWeek.Sunday ||
                   AttendanceDate.DayOfWeek == DayOfWeek.Saturday;
        }
    }

    public string SectionCode { get; set; }

    public string DepartmentCode { get; set; }

    public List<EmployeeAttendanceRequest> RequestList { get; set; }
}