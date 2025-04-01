namespace HRM.Criteria.Shift;

using Fanex.Data.Repository;

public class GetAllShiftCriteria : CriteriaBase
{
    public short CompanyId { get; set; }

    public override string GetSettingKey() => "Shift_GetShifts";

    public override bool IsValid() => CompanyId > 0;
}