namespace HRM.Criteria.Request;

using Fanex.Data;
using Core.Constants;
using Fanex.Data.Repository;

public class GetNexRequestHistoryCriteria : CriteriaBase
{
    public int RequestId { get; set; }

    [SpParam(Ignored = true)]
    public int? CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return CompanyId switch
        {
            Constants.NexCompanyId => "Nex_WorkingTimeRequest_GetRequestHistory",
            Constants.LumiCompanyId => "Lumi_WorkingTimeRequest_GetRequestHistory",
            Constants.NexVisionCompanyId => "NexVision_WorkingTimeRequest_GetRequestHistory",
            _ => "Nex_WorkingTimeRequest_GetRequestHistory"
        };
    }

    public override bool IsValid() => true;
}