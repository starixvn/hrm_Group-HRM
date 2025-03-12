namespace HRM.ViewModels.Unit;

using System.Collections.Generic;

public class UnitViewModel
{
    public bool? IsActive { get; set; }

    public IList<Models.Unit.Unit> Units { get; set; }

}