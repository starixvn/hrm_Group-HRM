namespace HRM.Criteria.Discipline;

using System;
using Fanex.Data;
using Fanex.Data.Repository;

public class DeleteWeekendLeaveCriteria : CriteriaBase
{
    [SpParam(SqlParamName = "EmpId")]
    public int EmployeeId { get; set; }

    public DateTime WorkDate { get; set; }

    public int ModifiedBy { get; set; }

    public override bool IsValid() => EmployeeId > 0 && ModifiedBy > 0 && WorkDate != default(DateTime);

    public override string GetSettingKey() => "EmployeeDiscipline_DeleteWeekendLeave";
}