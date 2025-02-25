namespace HRM.ViewModels.Department
{
    using System.ComponentModel.DataAnnotations;

    public class EditDepartmentViewModel : CreateDepartmentViewModel
    {
        [Required]
        public int DepartmentId { get; set; }
    }
}
