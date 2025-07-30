using System;

namespace HRM.Models.PayrollManagement;

public class PayrollEmployeeAttendant
{
    public int EmpId { get; set; }
    public int TotalUnpaidLeaveDays { get; set; }
    public int TotalNewHireWorkingDays { get; set; }
    public int TotalWorkingDays { get; set; }
    public int WorkingDays { get; set; }
    public bool IsResigned { get; set; }
    public bool IsMaternity { get; set; }
    public DateTime? JoinDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public string Salary { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal BasicSalaryPerDay { get => BasicSalary / WorkingDays; }
}