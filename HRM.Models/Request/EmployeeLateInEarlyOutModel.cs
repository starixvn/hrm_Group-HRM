namespace HRM.Models.Request;

using System;

public class EmployeeLateInEarlyOutModel
{
    public Employee Employee { get; set; }

    public DateTime WorkDate { get; set; }

    public int LateIn { get; set; }

    public int EarlyOut { get; set; }
}