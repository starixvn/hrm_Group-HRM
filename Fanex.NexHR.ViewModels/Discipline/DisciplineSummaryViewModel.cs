namespace Fanex.NexHR.ViewModels.Discipline
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Discipline;
    using System.Web.Mvc;

    public class DisciplineSummaryViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<Team> Teams { get; set; }

        [Display(Name = "Team")]
        public string TeamId { get; set; }

        public string TeamName { get; set; }

        public bool? IsLocal { get; set; }

        public string Employee { get; set; }

        public int? EmployeeId { get; set; }

        [Display(Name = "Reason")]
        public int? ReasonId { get; set; }

        public IList<DisciplineTypeModel> DisciplineTypes { get; set; }

        [Display(Name = "Ignored Status")]
        public bool? HasIgnored { get; set; }

        public bool FromSummaryPage { get; set; }

        public bool IsShowAll { get; set; }

        public IList<SelectListItem> DateRange { get; set; }
    }
}