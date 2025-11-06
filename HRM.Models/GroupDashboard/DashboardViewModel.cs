using System.Collections.Generic;
using System.Linq;

namespace HRM.Models.GroupDashboard;

public class DashboardViewModel
{
    public int CurrentHeadcount { get; set; }
    public int Recruitment { get; set; }
    public int ProbationResign { get; set; }
    public int ConfirmedResign { get; set; }
    public int Companies { get; set; }
    public int AvgAge { get; set; }
    public int AvgYrsTenure { get; set; }
    public int Countries => HeadcountByCountry.Count();
    public int BusinessUnits => HeadcountByBU.Count();
    public IEnumerable<HeadcountModel> HeadcountByBU { get; set; }
    public IEnumerable<HeadcountModel> HeadcountByCountry { get; set; }
    public IEnumerable<CategoryModel> BuCategory { get; set; }
    public IEnumerable<HeadcountModel> EmployeeByAge { get; set; }
    public IEnumerable<HeadcountModel> SalaryExpenseByBU { get; set; }
    public IEnumerable<HeadcountModel> SalaryExpenseByBUCategory { get; set; }
}

public class HeadcountModel
{
    public string Label { get; set; }
    public decimal Value { get; set; }
}

public class CategoryModel
{
    public string Category { get; set; }
    public int EmployeeCount { get; set; }
    public int BUCount { get; set; }
}