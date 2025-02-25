namespace Fanex.NexHR.ViewModels
{
    using Fanex.NexHR.Models;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class UserManagementViewModel : PagerOptionModel
    {
        [AllowHtml]
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
}