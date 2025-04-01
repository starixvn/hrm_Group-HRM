namespace HRM.Criteria.Discipline;

using System;
using Fanex.Data;
using Fanex.Data.Repository;

public class GetOtherDisciplineOfEmployeeCriteria : CriteriaBase
{
    [SpParam(SqlParamName = "EmpId")]
    public int EmployeeId { get; set; }

    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public override bool IsValid() => EmployeeId > 0 && FromDate <= ToDate;

    public override string GetSettingKey() => "OtherDiscipline_Get";
}