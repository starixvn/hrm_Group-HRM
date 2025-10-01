namespace HRM.ViewModels.UI;

using GO.UserControl.Client.Core.Models;
using System.Collections.Generic;
using HRM.Core.Models;

public class HeaderViewModel
{
    public EmployeeSession Employee { get; set; }
    public bool IsShowSearchBox { get; set; }
    public byte SiteId { get; set; }
    public IEnumerable<Site> UserSites { get; set; }
}