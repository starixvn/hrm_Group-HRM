namespace HRM.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    public class GetSupervisorByRequestTypeCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public byte Role { get; set; }

        public byte RequestType { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetSupervisorForRequest";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}