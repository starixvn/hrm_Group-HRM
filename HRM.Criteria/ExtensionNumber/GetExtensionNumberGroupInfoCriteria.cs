namespace HRM.Criteria.ExtensionNumber;

using Fanex.Data.Repository;

public class GetExtensionNumberGroupInfoCriteria : CriteriaBase
{
    public bool? IsInternal { get; set; }

    public int? TeamId { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey() => "ExtensionNO_GetGroupInfo";

    public override bool IsValid() => true;
}