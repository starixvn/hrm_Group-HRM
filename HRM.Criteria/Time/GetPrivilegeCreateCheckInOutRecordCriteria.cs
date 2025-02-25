namespace HRM.Criteria.Time
{
    using Fanex.Data.Repository;

    public class GetPrivilegeCreateCheckInOutRecordCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey()
        {
            return "TimeRecord_GetPrivilegeCreateCheckInOut";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}