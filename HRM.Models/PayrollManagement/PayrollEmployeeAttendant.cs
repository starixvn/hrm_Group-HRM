using System;

namespace HRM.Models.PayrollManagement;

public class PayrollEmployeeAttendant
{
    public int EmpId { get; set; }
    public decimal TotalUnpaidLeaveDays { get; set; }
    public decimal TotalNewHireWorkingDays { get; set; }
    public decimal TotalWorkingDays { get; set; }
    public int WorkingDays { get; set; }
    public bool IsResigned { get; set; }
    public bool IsMaternity { get; set; }
    public DateTime? CompanyDate { get; set; }
    public DateTime? JoinedDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public string Salary { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal BasicSalaryPerDay { get => BasicSalary / WorkingDays; }
    public decimal ActualPaidDays { get; set; }

}