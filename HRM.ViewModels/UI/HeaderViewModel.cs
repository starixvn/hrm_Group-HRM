namespace HRM.ViewModels.UI
{
    using HRM.Core.Models;

    public class HeaderViewModel
    {
        public EmployeeSession Employee { get; set; }
        public bool IsShowSearchBox { get; set; }
        public byte SiteId { get; set; }
    }
}