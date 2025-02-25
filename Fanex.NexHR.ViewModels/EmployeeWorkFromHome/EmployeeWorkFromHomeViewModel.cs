namespace Fanex.NexHR.ViewModels.EmployeeWorkFromHome
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EmployeeWorkFromHomeViewModel
    {
        public long RequestId { get; set; }
        public int EmployeeWorkFromHomeId { get; set; }

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

        public string Remark { get; set; }

        public List<string> Files { get; set; }
    }
}