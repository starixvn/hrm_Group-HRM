namespace HRM.ViewModels.EmployeeGroup
{
    public class UpdateEmployeeViewModel
    {
        public int EmpId { get; set; }
        public int? CtrId { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public string CompanyName { get; set; }
    }
}