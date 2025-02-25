namespace Fanex.NexHR.ViewModels.Unit
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class UnitViewModel
    {
        public bool? IsActive { get; set; }

        public IList<Unit> Units { get; set; }

    }
}
