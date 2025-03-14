namespace HRM.AutoJobs.Criteria;

using Fanex.Data.Repository;

public class InsertAnnualLeaveBalanceForNewEmployeeCommand : NonQueryCommand
{
    public int Year { get; set; }
    public int? EmpId { get; set; }

    public override string GetSettingKey() => "Auto_AnnualLeaveBalance_UpdateForNewEmployee";

    public override bool IsValid() => Year > 0;
}