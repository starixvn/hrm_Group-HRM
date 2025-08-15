using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRM.ViewModels.EmployeeGroup;

public class GroupEmployeeViewModel : PagerOptionModel
{
    public string Keyword { get; set; }

    public string TeamIds { get; set; } = "";

    public string CompanyIds { get; set; } = "";

    public int? JobTitleId { get; set; }

    public bool? IsEnable { get; set; }

    public bool? IsDelete { get; set; }

    public bool? IsAll { get; set; }

    public IList<Models.Team> Groups { get; set; }

    public IEnumerable<SelectListItem> JobTitles { get; set; }

    public bool IsShowDropdownCategory { get; set; }

    public string CtrIDs { get; set; } = "";

    public string Outsources { get; set; } = "";

    public string EmployeeStatuses { get; set; } = "";
}