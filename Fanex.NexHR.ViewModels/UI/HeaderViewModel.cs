namespace Fanex.NexHR.ViewModels.UI
{
    using Fanex.NexHR.Core.Models;

    public class HeaderViewModel
    {
        public EmployeeSession Employee { get; set; }
        public bool IsShowSearchBox { get; set; }
        public byte SiteId { get; set; }
    }
}