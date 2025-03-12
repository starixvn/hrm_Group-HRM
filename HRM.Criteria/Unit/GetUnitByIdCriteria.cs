namespace HRM.Criteria.Unit;

using Fanex.Data.Repository;

public class GetUnitByIdCriteria : CriteriaBase
{
    public int UnitId { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey() => "Unit_GetById";

    public override bool IsValid() => UnitId > 0;
}