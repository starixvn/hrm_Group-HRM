namespace HRM.ViewModels.UI;

using System.Collections.Generic;
using HRM.ViewModels.UserSite;
using HRM.Core.Models;

public class HeaderViewModel
{
    public EmployeeSession Employee { get; set; }
    public bool IsShowSearchBox { get; set; }
    public byte SiteId { get; set; }
    public IEnumerable<UserSite> UserSites { get; set; }
}

