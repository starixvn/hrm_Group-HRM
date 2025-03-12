namespace HRM.AutoJobs.Criteria.GroupReport;

using System;
using Fanex.Data.Repository;

public class GetEmployeeInfoCriteria : CriteriaBase
{
    public DateTime? LastModifiedDate { get; set; }

    public override string GetSettingKey() => "HRM_SyncData_GroupReport_GetEmployeeInfo";

    public override bool IsValid() => true;
}