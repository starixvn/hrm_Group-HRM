namespace Fanex.NexHR.ViewModels.Report
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Report;

    public class ManpowerCategoryViewModel
    {
        public int Year { get; set; }

        public IEnumerable<ManpowerCategoryModel> Manpowers { get; set; }
    }
}