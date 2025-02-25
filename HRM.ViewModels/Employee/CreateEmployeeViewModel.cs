namespace HRM.ViewModels.Employee
{
    using System.ComponentModel.DataAnnotations;

    public class CreateEmployeeViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Nickname { get; set; }

        public string DomainUsername { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public int TeamId { get; set; }
    }
}