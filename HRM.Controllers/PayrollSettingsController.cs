using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Criteria.PayrollManagement;
using HRM.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class PayrollSettingsController(
    IUserControlSession session,
    IPayrollSettingRepository repository) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> Index()
    {
        var criteria = new GetPayrollItemSettingsCriteria
        {
            CompanyId = CurrentCompanyId
        };

        var result = await repository.GetPayrollItemSettings(criteria);

        return View(result);
    }
}