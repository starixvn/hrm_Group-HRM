namespace HRM.ViewModels.Unit
{
    using System.Collections.Generic;
    using HRM.Models;

    public class UnitViewModel
    {
        public bool? IsActive { get; set; }

        public IList<Unit> Units { get; set; }

    }
}
