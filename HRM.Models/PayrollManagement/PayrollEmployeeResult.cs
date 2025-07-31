using System;
using HRM.Core.Enums;

namespace HRM.Models.PayrollManagement;

public class PayrollEmployeeResult
{
    public int EmpId { get; set; }
    public int WorkingDays { get; set; }
    public decimal PaidDays { get; set; }
    public decimal UnpaidLeave { get; set; }
    public string BasicSalary { get; set; }
    public string GrossSalary { get; set; }
    public DateTime? JoinDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public EmploymentStatus EmploymentStatus { get; set; }
}