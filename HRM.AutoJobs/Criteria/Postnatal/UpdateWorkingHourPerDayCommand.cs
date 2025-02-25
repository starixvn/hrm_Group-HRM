namespace HRM.AutoJobs.Criteria.Postnatal
{
    using Fanex.Data.Repository;

    public class UpdateWorkingHourPerDayCommand : NonQueryCommand
    {
        public int? EmpId { get; set; }

        public override string GetSettingKey() => "Auto_Postnatal_UpdateWorkingHour";

        public override bool IsValid() => true;
    }
}