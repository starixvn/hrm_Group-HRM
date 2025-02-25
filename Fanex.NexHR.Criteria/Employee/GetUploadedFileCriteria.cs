namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetUploadedFileCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetUploadedFile";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}