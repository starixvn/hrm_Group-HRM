namespace HRM.Models;

public class EmployeeShift
{
    public int EmpId { get; set; }

    public int? MonShiftId { get; set; }

    public string MonShiftName { get; set; }

    public int? TueShiftId { get; set; }

    public string TueShiftName { get; set; }

    public int? WedShiftId { get; set; }

    public string WedShiftName { get; set; }

    public int? ThuShiftId { get; set; }

    public string ThuShiftName { get; set; }

    public int? FriShiftId { get; set; }

    public string FriShiftName { get; set; }

    public int? SatShiftId { get; set; }

    public string SatShiftName { get; set; }

    public int? SunShiftId { get; set; }

    public string SunShiftName { get; set; }
}