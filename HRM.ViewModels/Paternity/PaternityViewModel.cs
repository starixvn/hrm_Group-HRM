namespace HRM.ViewModels.Paternity
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PaternityViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "The Employee is required")]
        public int EmployeeId { get; set; }

        [Display(Name = "Employee")]
        public string Employee { get; set; }

        [Required(ErrorMessage = "From Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Child's Born")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ChildBornDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Leave day is required")]
        public int LeaveDay { get; set; }

        public string Remark { get; set; }
    }
}