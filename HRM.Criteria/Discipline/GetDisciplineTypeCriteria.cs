namespace HRM.Criteria.Discipline;

using Fanex.Data.Repository;

public class GetDisciplineTypeCriteria : CriteriaBase
{
    public bool IncludeOtherType { get; set; }

    public override bool IsValid()
    {
        return true;
    }

    public override string GetSettingKey()
    {
        return "DisciplineType_GetDisciplineType";
    }
}