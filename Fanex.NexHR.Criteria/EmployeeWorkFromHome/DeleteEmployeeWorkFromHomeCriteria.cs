namespace Fanex.NexHR.Criteria.EmployeeWorkFromHome
{
    using Fanex.Data.Repository;

    public class DeleteEmployeeWorkFromHomeCriteria : CriteriaBase
    {
        public int EmployeeWorkFromHomeId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeWorkFromHome_Delete";
        }

        public override bool IsValid()
        {
            return EmployeeWorkFromHomeId > 0;
        }
    }
}