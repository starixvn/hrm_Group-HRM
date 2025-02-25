namespace Fanex.NexHR.Criteria.Discipline
{
    using Fanex.Data.Repository;

    public class GetOtherDisciplineTypeCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "DisciplineType_GetOtherDisciplineType";
        }
    }
}