using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.User;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Security;
using HRM.Models;
using HRM.Services;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers;

public class AccountController(IUserControlSession userControlSession,
    IEmployeeService employeeService,
    ICryptoService cryptoService,
    IUserService userService) : BaseController(userControlSession)
{
    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public ActionResult ResetPassword(EditPassword model)
    {
        model.Password = Encryptor.ComputeHash("123456");
        model.IsReset = true;
        var commandResult = employeeService.EditPassword(EmployeeSession.EmpId, model);
        if (!commandResult.IsSuccessful) return Json(commandResult);

        var token = cryptoService.GenerateRijndaelKeyIV();
        var keyIv = token.Split(':');
        var rijndaelKey = keyIv[0];
        var rijndaelIv = keyIv[1];
        var result = userService.ResetPassword(new UpdatePasswordParam
        {
            UserId = model.UserId,
            Password = cryptoService.Encrypt("123456", rijndaelKey, rijndaelIv),
            Token = token
        });
        return Json(result);
    }
}