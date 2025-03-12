namespace HRM.ViewModels;

using System;
using System.ComponentModel.DataAnnotations;

public class EmployeeAttendanceViewModel
{
    [DataType(DataType.Date)]
    public DateTime FromDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime ToDate { get; set; }

    [Display(Name = "Employee")]
    public int? EmployeeId { get; set; }

    public string Employee { get; set; }

    public string TeamIds { get; set; }

    public short AttendanceType { get; set; }

    public int DateOption { get; set; }

    public bool ExcludeDeletedUsers { get; set; }

    public bool IsMember { get; set; }

    public string TeamName { get; set; }

    public byte? IsLocal { get; set; }

    public bool? IsRealDate { get; set; }

    public int SupervisorId { get; set; }

    public string Sort { get; set; }
}