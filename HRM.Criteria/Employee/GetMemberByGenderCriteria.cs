namespace HRM.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetMemberByGenderCriteria : CriteriaBase
    {
        public string Query { get; set; }

        public int Gender { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetMemberByGender";
        }
    }
}