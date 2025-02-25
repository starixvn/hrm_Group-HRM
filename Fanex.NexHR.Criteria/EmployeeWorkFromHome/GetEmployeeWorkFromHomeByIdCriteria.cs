namespace Fanex.NexHR.Criteria.EmployeeWorkFromHome
{
    using Fanex.Data.Repository;

    public class GetEmployeeWorkFromHomeByIdCriteria : CriteriaBase
    {
        public int EmployeeWorkFromHomeId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeWorkFromHome_GetById";
        }

        public override bool IsValid()
        {
            return EmployeeWorkFromHomeId > 0;
        }
    }
}