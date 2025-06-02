namespace HRM.ViewModels.CompanySecondmentEmployee;

using System.ComponentModel.DataAnnotations;

public class UpdateCompanySecondEmployeeViewModel : CreateCompanySecondEmployeeViewModel
{
    public int OldEmpId { get; set; }
}