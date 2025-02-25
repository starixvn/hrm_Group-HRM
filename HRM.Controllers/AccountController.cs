using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Filters;
    using HRM.Core.Security;
    using HRM.Models;
    using HRM.Resources;
    using HRM.Services;
    using HRM.ViewModels.Employee;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Models.User;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;

    public class AccountController : BaseController
    {
        private readonly IHRMAuthService authService;
        private readonly IEmployeeService employeeService;
        private readonly IEmployeeLogService employeeLogService;
        private readonly ICryptoService cryptoService;
        private readonly IUserService userService;
        private readonly ICommonDataService commonDataService;

        public AccountController(
            IUserControlSession userControlSession,
            IHRMAuthService authService,
            IEmployeeService employeeService,
            IEmployeeLogService employeeLogService,
            ICryptoService cryptoService,
            IUserService userService,
            ICommonDataService commonDataService) : base(userControlSession)
        {
            this.authService = authService;
            this.employeeService = employeeService;
            this.employeeLogService = employeeLogService;
            this.cryptoService = cryptoService;
            this.userService = userService;
            this.commonDataService = commonDataService;
        }

        public ActionResult Index()
        {
            var viewModel = new AccountViewModel
            {
                Employee = employeeService.GetEmployeeById(EmployeeSession.EmpId),
                Logs = employeeLogService.GetLogDetailById(EmployeeSession.EmpId)
            };
            ViewBag.EnableGroupDate = commonDataService.IsEnableGroupDate();

            return View(viewModel);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult ResetPassword(EditPassword model)
        {
            model.Password = Encryptor.ComputeHash("123456");
            model.IsReset = true;
            var commandResult = employeeService.EditPassword(EmployeeSession.EmpId, model);
            if (commandResult.IsSuccessful)
            {
                var token = cryptoService.GenerateRijndaelKeyIV();
                string[] keyiv = token.Split(':');
                var rijndaelKey = keyiv[0];
                var rijndaelIV = keyiv[1];
                var result = userService.ResetPassword(new UpdatePasswordParam
                {
                    UserId = model.UserId,
                    Password = cryptoService.Encrypt("123456", rijndaelKey, rijndaelIV),
                    Token = token
                });
                return Json(result);
            }

            return Json(commandResult);
        }

        [HttpGet]
        public ActionResult ChangePwd()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePwd(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var retCode = authService.Validate(EmployeeSession, model.Password);
                if (retCode == (short)ValidateUserStatus.Success)
                {
                    var param = new EditPassword
                    {
                        EmployeeId = EmployeeSession.EmpId,
                        Password = Encryptor.ComputeHash(model.NewPassword)
                    };
                    var commandResult = employeeService.EditPassword(EmployeeSession.EmpId, param);
                    switch (commandResult.Code)
                    {
                        case (short)ValidateUserStatus.Success: // OK
                            UpdatePassword(model);
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
            }

            return View(model);
        }

        private void UpdatePassword(ChangePasswordViewModel model)
        {
            var token = cryptoService.GenerateRijndaelKeyIV();
            string[] keyiv = token.Split(':');
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            var commandResult = userService.UpdatePassword(new UpdatePasswordParam
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
}