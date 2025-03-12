namespace HRM.ViewModels.Section
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Models;

    public class AddSectionViewModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;

        public IList<Department> Departments { get; set; }
    }
}