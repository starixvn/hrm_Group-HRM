namespace HRM.ViewModels.EmployeeWorkFromHome;

using System;

public class EmployeeWorkFromHomeFilterViewModel
{
    public DateTime FromDateFilter { get; set; }
    public DateTime ToDateFilter { get; set; }
    public string TeamIds { get; set; }
    public byte? IsLocal { get; set; }
    public byte? IsPermanent { get; set; }
}
