namespace HRM.Criteria.SyncData;

using Fanex.Data.Repository;

public class ImportWorkingShiftCommand : CriteriaBase
{
    public string WorkingShiftXml { get; set; }

    public override string GetSettingKey() => "Sync_WorkingShift_Import";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(WorkingShiftXml);
}