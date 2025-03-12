namespace HRM.ViewModels;

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

public class UserManagementViewModel : PagerOptionModel
{
    public string Keyword { get; set; }

    public string TeamIds { get; set; }

    public int? JobTitleId { get; set; }

    public bool? IsEnable { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsLocal { get; set; }

    public IList<Models.Team> Groups { get; set; }

    public IList<SelectListItem> JobTitles { get; set; }

    public bool IsShowDropdownCategory { get; set; }
}