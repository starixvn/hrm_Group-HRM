namespace Fanex.NexHR.Criteria.Shift
{
    using Fanex.Data.Repository;

    public class EditShiftCriteria : CriteriaBase
    {
        public int ShiftId { get; set; }

        public string Name { get; set; }

        public string CheckInTime { get; set; }

        public string CheckOutTime { get; set; }

        public bool IsInAfterDay { get; set; }

        public bool IsOutAfterDay { get; set; }

        public bool IsDisabled { get; set; }

        public int ActorId { get; set; }

        public override string GetSettingKey() => "Shift_Update";

        public override bool IsValid() => ShiftId >= 0 && !string.IsNullOrWhiteSpace(Name);
    }
}