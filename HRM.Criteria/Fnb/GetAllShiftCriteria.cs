using Fanex.Data.Repository;

namespace HRM.Criteria.Fnb;

public class GetAllShiftCriteria : CriteriaBase
{
    public short CompanyId { get; set; }

    public override string GetSettingKey() => "FnB_Shift_GetAll";

    public override bool IsValid() => true;
}