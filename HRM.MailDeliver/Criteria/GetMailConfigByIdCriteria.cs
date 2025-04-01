namespace HRM.MailDeliver.Criteria;

using Fanex.Data.Repository;

public class GetMailConfigByIdCriteria : CriteriaBase
{ 
    public int AppId { get; set; }

    public override string GetSettingKey() => "MailRecorder_GetConfig";

    public override bool IsValid() => AppId > 0;
}