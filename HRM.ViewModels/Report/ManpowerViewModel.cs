namespace HRM.ViewModels.Report;

using System.Collections.Generic;
using HRM.Models.Report;

public class ManpowerViewModel
{
    public int Year { get; set; }

    public IEnumerable<ManpowerModel> Manpowers { get; set; }
}