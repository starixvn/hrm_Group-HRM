namespace HRM.Criteria.Postnatal;

using Fanex.Data.Repository;

public class DeletePostnatalCriteria : CriteriaBase
{
    public int PostnatalId { get; set; }

    public override string GetSettingKey()
    {
        return "Postnatal_Delete";
    }

    public override bool IsValid()
    {
        return PostnatalId > 0;
    }
}