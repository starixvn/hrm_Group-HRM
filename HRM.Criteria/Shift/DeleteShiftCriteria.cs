namespace HRM.Criteria.Shift
{
    using Fanex.Data.Repository;

    public class DeleteShiftCriteria : CriteriaBase
    {
        public int ShiftId { get; set; }

        public override string GetSettingKey() => "Shift_Delete";

        public override bool IsValid() => ShiftId > 0;
    }
}