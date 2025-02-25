namespace Fanex.NexHR.Criteria.Shift
{
    using Fanex.Data.Repository;

    public class CreateShiftCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public string CheckInTime { get; set; }

        public string CheckOutTime { get; set; }

        public bool IsInAfterDay { get; set; }

        public bool IsOutAfterDay { get; set; }

        public int ActorId { get; set; }

        public override string GetSettingKey() => "Shift_Insert";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name);
    }
}