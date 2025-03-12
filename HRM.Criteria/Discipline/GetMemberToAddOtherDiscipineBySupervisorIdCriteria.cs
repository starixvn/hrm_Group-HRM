namespace HRM.Criteria.Discipline;

using Fanex.Data.Repository;

public class GetMemberToAddOtherDiscipineBySupervisorIdCriteria : CriteriaBase
{
    public string Query { get; set; }

    public int SupervisorId { get; set; }

    public override bool IsValid()
    {
        return SupervisorId > 0;
    }

    public override string GetSettingKey()
    {
        return "OtherDiscipline_GetMember";
    }
}