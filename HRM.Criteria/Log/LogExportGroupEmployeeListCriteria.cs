using System;
using Fanex.Data.Repository;

namespace HRM.Criteria.Log;

public class LogExportGroupEmployeeListCriteria : CriteriaBase
{
    public int UserId { get; set; }

    public int ObjectTypeId { get; set; }

    public override string GetSettingKey() => "ActivityLog_Insert";
    public override bool IsValid() => true;
}

