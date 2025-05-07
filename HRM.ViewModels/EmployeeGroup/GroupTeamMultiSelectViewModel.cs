using System.Collections.Generic;

namespace HRM.ViewModels.EmployeeGroup;

public class GroupTeamMultiSelectViewModel
{
    public IEnumerable<MultiSelectViewModel> Data { get; set; }

    public string TeamIds { get; set; }
}