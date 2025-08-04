using Fanex.Data;
using HRM.Core;
using System;
using System.Text.Json.Serialization;

namespace HRM.Models.Payroll;

public class GeneratedPayrollDetail
{
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string FullName { get; set; }
    public string NickName { get; set; }
    [JsonIgnore]
    public DateTime JoinedDate { get; set; }
    public string DisplayJoinedDate => JoinedDate.ToDateMonthYearFormatedDate();
    [JsonIgnore]
    public DateTime? ConfirmedDate { get; set; }
    public string DisplayConfirmedDate => ConfirmedDate.ToDateMonthYearFormatedDate();
    [JsonIgnore]
    public string BasicSalaryEncrypt { get; set; }
    public double BasicSalary => BasicSalaryEncrypt == null ? 0 : double.Parse(Cryptography.DecryptPassword(BasicSalaryEncrypt));
    public int StandardWorkingDay { get; set; }
    public decimal UnpaidLeave { get; set; }
    public decimal PaidDay { get; set; }
    [JsonIgnore]
    public string GrossSalaryEncrypt { get; set; }
    public double GrossSalary => double.Parse(Cryptography.DecryptPassword(GrossSalaryEncrypt));
    public int EmploymentStatus { get; set; }
}