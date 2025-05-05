using System;
using Microsoft.AspNetCore.Http;

namespace HRM.Models.Payroll;

public class EmployeePayroll
{
    public int EmpId { get; set; }
    public string FullName { get; set; }
    public string Nickname { get; set; }
    public string DepartmentName { get; set; }
    public string TeamName { get; set; }
    public DateTime Month { get; set; }
    public decimal PersonalCost { get; set; }
    public decimal ExtraPayment { get; set; }
}
