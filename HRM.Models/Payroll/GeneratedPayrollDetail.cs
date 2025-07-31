using Fanex.Data;
using System;
using System.Text.Json.Serialization;

namespace HRM.Models.Payroll;

public class GeneratedPayrollDetail
{
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string FullName { get; set; }
    public string NickName { get; set; }
    public DateTime JoinedDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    [JsonIgnore]
    public string BasicSalaryEncrypt { get; set; }
    public double BasicSalary => double.Parse(Cryptography.DecryptPassword(BasicSalaryEncrypt));
    public int StandardWorkingDay { get; set; }
    public int UnpaidLeave { get; set; }
    public int PaidDay { get; set; }
    [JsonIgnore]
    public string GrossSalaryEncrypt { get; set; }
    public double GrossSalary => double.Parse(Cryptography.DecryptPassword(GrossSalaryEncrypt));
}