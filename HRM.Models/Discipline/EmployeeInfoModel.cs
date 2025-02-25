namespace HRM.Models.Discipline
{
    public class EmployeeInfoModel
    {
        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public bool IsLocal { get; set; }

        public int TeamId { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }
    }
}