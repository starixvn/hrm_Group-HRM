namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetUserLoginByDomainUserNameCriteria : CriteriaBase
    {
        public string DomainUsername { get; set; }

        public override string GetSettingKey()
        {
            return "GetEmployeeByDomainUserName";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(DomainUsername);
        }
    }
}
