namespace HRM.Models
{
    public class CreateEmployeeParam
    {
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string DomainUsername { get; set; }
        public string Code { get; set; }
        public int TeamId { get; set; }
    }
}