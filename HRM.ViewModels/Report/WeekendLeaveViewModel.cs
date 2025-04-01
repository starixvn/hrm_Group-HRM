namespace HRM.ViewModels.Report;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models;
using Microsoft.AspNetCore.Mvc.Rendering;

public class WeekendLeaveViewModel
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

    [Display(Name = "Leave Type")]
    public int LeaveTypeId { get; set; }

    public IList<SelectListItem> DateRange { get; set; }

    [Display(Name = "Total Leave >=")]
    public int MinTotalLeave { get; set; }
}