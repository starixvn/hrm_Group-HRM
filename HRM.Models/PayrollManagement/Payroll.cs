namespace HRM.Models.PayrollManagement;

public class Payroll
{
    public int EmpId { get; set; }
    public string EncryptBasicSalary { get; set; }
    public string EncryptExpatSalary { get; set; }
    public string EncryptJobLevelAddOn { get; set; }
    public string EncryptSpecialSubsidy { get; set; }
}

public class LocalPayrollAllowance
{
    public int EmpId { get; set; }
    public string Type { get; set; }
    public bool Taxable { get; set; }
    public string ItemName { get; set; }
    public string EncryptAmount { get; set; }
}