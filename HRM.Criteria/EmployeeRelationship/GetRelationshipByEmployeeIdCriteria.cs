namespace HRM.Criteria.EmployeeRelationship;

using Fanex.Data.Repository;

public class GetRelationshipByEmployeeIdCriteria : CriteriaBase
{
    public int EmployeeId { get; set; }

    public override string GetSettingKey() => "EmployeeRelationship_GetByEmployeeId";

    public override bool IsValid() => EmployeeId > 0;
}