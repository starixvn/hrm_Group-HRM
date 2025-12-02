using Fanex.Data;
using HRM.Core;
using System;
using System.Text.Json.Serialization;

namespace HRM.Models.Payroll;

public class GeneratedPayrollDetail
{
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string BUCode { get; set; }
    public string DepartmentCode { get; set; }
    public string TeamCode { get; set; }
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
    [JsonIgnore]
    public string SpecialSubsidyEncrypt { get; set; }
    public double SpecialSubsidy => SpecialSubsidyEncrypt == null ? 0 : double.Parse(Cryptography.DecryptPassword(SpecialSubsidyEncrypt));
    public string TotalIncomeEncrypt { get; set; }
    [JsonIgnore]
    public double TotalIncome => TotalIncomeEncrypt == null ? 0 : double.Parse(Cryptography.DecryptPassword(TotalIncomeEncrypt));
    public string JobLevelAddOnEncrypt { get; set; }
    [JsonIgnore]
    public double JobLevelAddOn => JobLevelAddOnEncrypt == null ? 0 : double.Parse(Cryptography.DecryptPassword(JobLevelAddOnEncrypt));
    public string TransportationAllowanceEncrypt { get; set; }
    [JsonIgnore]
    public double TransportationAllowance => TransportationAllowanceEncrypt == null ? 0 : double.Parse(Cryptography.DecryptPassword(TransportationAllowanceEncrypt));
    public string ManagementAllowanceEncrypt { get; set; }
    [JsonIgnore]
    public double ManagementAllowance => ManagementAllowanceEncrypt == null ? 0 : double.Parse(Cryptography.DecryptPassword(ManagementAllowanceEncrypt));
    public string MobilephoneAllowanceEncrypt { get; set; }
    [JsonIgnore]
    public double MobilephoneAllowance => MobilephoneAllowanceEncrypt == null ? 0 : double.Parse(Cryptography.DecryptPassword(MobilephoneAllowanceEncrypt));
    public int EmploymentStatus { get; set; }
}