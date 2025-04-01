namespace HRM.Criteria.Paternity;

using Fanex.Data.Repository;

public class DeletePaternityCriteria : CriteriaBase
{
    public int PaternityId { get; set; }

    public override string GetSettingKey()
    {
        return "Paternity_Delete";
    }

    public override bool IsValid()
    {
        return PaternityId > 0;
    }
}