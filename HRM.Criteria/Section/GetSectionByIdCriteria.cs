namespace HRM.Criteria.Section;

using Fanex.Data.Repository;

public class GetSectionByIdCriteria : CriteriaBase
{
    public int SectionId { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey() => "Section_GetById";

    public override bool IsValid() => SectionId > 0;
}