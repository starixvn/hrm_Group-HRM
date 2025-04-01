namespace HRM.ViewModels.Report;

using System.Collections.Generic;
using HRM.Models.Report;

public class ManpowerCategoryViewModel
{
    public int Year { get; set; }

    public IEnumerable<ManpowerCategoryModel> Manpowers { get; set; }
}