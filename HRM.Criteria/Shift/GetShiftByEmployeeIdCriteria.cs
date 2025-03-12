namespace HRM.Criteria.Shift;

using Fanex.Data.Repository;

public class GetShiftByEmployeeIdCriteria : CriteriaBase
{
    public int EmpId { get; set; }

    public override string GetSettingKey() => "EmployeeShift_Get";

    public override bool IsValid() => EmpId > 0;
}