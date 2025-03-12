using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.User;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using HRM.Core.Security;
using HRM.Models;
using HRM.Resources;
using HRM.Services;
using HRM.Services.Implement;
using HRM.Services.Implement.ActionLog;
using HRM.Services.Interfaces;
using HRM.ViewModels.Employee;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class AccountController(
    IUserControlSession userControlSession,
    IEmployeeService employeeService,
    IEmployeeLogService employeeLogService,
    ICryptoService cryptoService,
    IHRMAuthService authService,
    ICommonDataService commonDataService,
    IUserService userService) : BaseController(userControlSession)
{
    public async Task<IActionResult> Index()
    {
        var viewModel = new AccountViewModel
        {
            Employee = await employeeService.GetEmployeeByIdAsync(EmployeeSession.EmpId),
            Logs = await employeeLogService.GetLogDetailByIdAsync(EmployeeSession.EmpId)
        };
        ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

        return View(viewModel);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> ResetPassword(EditPassword model)
    {
        model.Password = Encryptor.ComputeHash("123456");
        model.IsReset = true;
        var commandResult = await employeeService.EditPasswordAsync(EmployeeSession.EmpId, model);
        if (!commandResult.IsSuccessful)
        {
            return Json(commandResult);
        }

        var token = cryptoService.GenerateRijndaelKeyIV();
        var keyIv = token.Split(':');
        var rijndaelKey = keyIv[0];
        var rijndaelIv = keyIv[1];
        var result = await userService.ResetPasswordAsync(new UpdatePasswordParam
        {
            UserId = model.UserId,
            Password = cryptoService.Encrypt("123456", rijndaelKey, rijndaelIv),
            Token = token
        });
        return Json(result);
    }

    [HttpGet]
    public ActionResult ChangePwd()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePwd(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var retCode = authService.Validate(EmployeeSession, model.Password);
        if (retCode == (short)ValidateUserStatus.Success)
        {
            var param = new EditPassword
            {
                EmployeeId = EmployeeSession.EmpId,
                Password = Encryptor.ComputeHash(model.NewPassword)
            };
            var commandResult = await employeeService.EditPasswordAsync(EmployeeSession.EmpId, param);
            switch (commandResult.Code)
            {
                case (short)ValidateUserStatus.Success: // OK
                    await UpdatePassword(model);
                    break;

                case (short)ValidateUserStatus.Deleted:
                    ModelState.AddModelError(string.Empty, Messages.DeletedUser);
                    break;

                case (short)ValidateUserStatus.Disabled:
                    ModelState.AddModelError(string.Empty, Messages.DisabledUser);
                    break;

                default: // wrong password
                    ModelState.AddModelError(string.Empty, Messages.WrongPassword);
                    break;
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, Messages.WrongPassword);
        }

        return View(model);
    }

    private async Task UpdatePassword(ChangePasswordViewModel model)
    {
        var token = cryptoService.GenerateRijndaelKeyIV();
        var keyiv = token.Split(':');
        var rijndaelKey = keyiv[0];
        var rijndaelIV = keyiv[1];
        var commandResult = await userService.UpdatePasswordAsync(new UpdatePasswordParam
        {
            OldPassword = cryptoService.Encrypt(model.Password, rijndaelKey, rijndaelIV),
            Password = cryptoService.Encrypt(model.NewPassword, rijndaelKey, rijndaelIV),
            Token = token
        });

        if (commandResult.IsSuccessful)
        {
            ViewBag.ChangePwdOK = Messages.ChangePwdOK;
        }
        else
        {
            ModelState.AddModelError(string.Empty, commandResult.Message);
        }
    }
}