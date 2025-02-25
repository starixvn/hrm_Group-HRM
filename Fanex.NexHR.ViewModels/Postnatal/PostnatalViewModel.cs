namespace Fanex.NexHR.ViewModels.Postnatal
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Resources;

    public class PostnatalViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Employee is required")]
        public int EmployeeId { get; set; }

        [Display(Name = "Employee")]
        public string Employee { get; set; }

        [Required(ErrorMessage = "From Date is required.")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "To Date is required.")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Required(ErrorMessage = "Working hours per day is required.")]
        [Display(Name = nameof(Labels.WorkingHoursPerDay), ResourceType = typeof(Labels))]
        public byte? WorkingHours { get; set; }

        public string Remark { get; set; }
    }
}