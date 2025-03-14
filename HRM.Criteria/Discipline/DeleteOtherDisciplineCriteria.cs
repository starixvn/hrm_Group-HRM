namespace HRM.Criteria.Discipline;

using System;
using Fanex.Data;
using Fanex.Data.Repository;

public class DeleteOtherDisciplineCriteria : CriteriaBase
{
    [SpParam(SqlParamName = "EmpId")]
    public int EmployeeId { get; set; }

    public DateTime WorkDate { get; set; }
    public int ActorId { get; set; }

    public override bool IsValid() => EmployeeId > 0 && ActorId > 0;

    public override string GetSettingKey() => "OtherDiscipline_Delete";
}