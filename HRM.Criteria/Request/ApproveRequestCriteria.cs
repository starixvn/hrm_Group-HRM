namespace HRM.Criteria.Request;

using Fanex.Data.Repository;

public class ApproveRequestCriteria : CriteriaBase
{
    public long RequestId { get; set; }
    public int ActorId { get; set; }
    public byte RoleId { get; set; }
    public string Comment { get; set; }
    public string ViewType { get; set; }
    public string DisciplineXml { get; set; }

    public override string GetSettingKey() => "WorkingTimeRequest_ApproveRequest";

    public override bool IsValid() => RequestId > 0 && ActorId > 0;
}