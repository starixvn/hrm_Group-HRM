using System;
using Fanex.Data.Repository;

namespace BIO.WebAPI.Criteria;

public class GetTimeRecordCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "API_TimeRecord_Get";

    public override bool IsValid() => true;
}