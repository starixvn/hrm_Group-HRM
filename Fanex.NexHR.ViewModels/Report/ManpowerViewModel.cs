namespace Fanex.NexHR.ViewModels.Report
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Report;

    public class ManpowerViewModel
    {
        public int Year { get; set; }

        public IEnumerable<ManpowerModel> Manpowers { get; set; }
    }
}