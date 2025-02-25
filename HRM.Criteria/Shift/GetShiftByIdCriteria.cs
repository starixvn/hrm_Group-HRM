namespace HRM.Criteria.Shift
{
    using Fanex.Data.Repository;

    public class GetShiftByIdCriteria : CriteriaBase
    {
        public int ShiftId { get; set; }

        public override string GetSettingKey() => "Shift_GetById";

        public override bool IsValid() => ShiftId >= 0;
    }
}