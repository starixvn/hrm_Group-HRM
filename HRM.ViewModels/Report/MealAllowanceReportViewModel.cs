namespace HRM.ViewModels.Report
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRM.Models;

    public class MealAllowanceReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public byte? IsLocal { get; set; }
        public string TeamIds { get; set; }

        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }
        public string Employee { get; set; }
        public IEnumerable<EntityBase<int>> Teams { get; set; }
    }
}
