namespace HRM.AutoJobs.Criteria
{
    using Fanex.Data.Repository;

    public class ImportTimeRecordCommand : NonQueryCommand
    {
        public override string GetSettingKey() => "TimeRecord_ImportData";

        public override bool IsValid() => true;
    }
}