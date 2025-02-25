namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Group;
    using Fanex.NexHR.Models.Log;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Employee;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Models.Role;
    using GO.UserControl.Client.Core.Models.User;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using Simplicode.Imaging;

    public class EmployeeController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly ICommonDataService commonDataService;
        private readonly IEmployeeService employeeService;
        private readonly IHRMAuthService authService;
        private readonly IEmployeeLogService employeeLogService;
        private readonly ITeamService teamService;
        private readonly IJobLevelService jobLevelService;
        private readonly ICryptoService cryptoService;
        private readonly IUserService userService;
        private readonly IRoleService roleService;

#pragma warning disable S107 // Methods should not have too many parameters
        public EmployeeController(
            IUserControlSession userControlSession,
            IEmployeeService employeeService,
            ICommonDataService commonDataService,
            IHRMAuthService authService,
            IEmployeeLogService employeeLogService,
            ITeamService teamService,
            IJobLevelService jobLevelService,
            ICryptoService cryptoService,
            IUserService userService,
            IRoleService roleService) : base(userControlSession)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.userControlSession = userControlSession;
            this.employeeService = employeeService;
            this.commonDataService = commonDataService;
            this.authService = authService;
            this.employeeLogService = employeeLogService;
            this.teamService = teamService;
            this.jobLevelService = jobLevelService;
            this.cryptoService = cryptoService;
            this.userService = userService;
            this.roleService = roleService;
        }

        [HRMPermission(Policy.Read, FunctionKey.ContactList)]
        public ActionResult Index()
        {
            var model = new UserManagementViewModel
            {
                IsEnable = true,
                CurrentPage = Constants.CurrentPage,
                ItemsPerPage = Constants.ItemsPerPage,
                TeamIds = string.Join(",", teamService.GetAll().Select(x => x.Id))
            };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.ContactList)]
        public ActionResult ContactList(UserManagementViewModel model)
        {
            IList<Employee> employees = new List<Employee>();
            if (model != null)
            {
                var criteria = new GetAllEmployeeCriteria
                {
                    Query = model.Keyword,
                    TeamIds = model.TeamIds,
                    EnableUser = model.IsEnable,
                    IsLocalUser = model.IsLocal,
                    PageNumber = model.CurrentPage,
                    PageSize = model.ItemsPerPage
                };

                employees = employeeService.GetEmployees(criteria);

                var pagerOption = new PagerOptionModel
                {
                    CurrentPage = model.CurrentPage,
                    ItemsPerPage = model.ItemsPerPage,
                    TotalItem = (int)criteria.TotalRow
                };
                ViewBag.PagerOption = pagerOption;
                ViewBag.IsShowJobtitle = roleService.GetByUserId(userControlSession.UserId)
                    .Any(r => r.RoleId == (short)RoleSystem.HR || r.RoleId == (short)RoleSystem.SystemAdmin);
                ViewBag.EnableGroupDate = commonDataService.IsEnableGroupDate();
            }

            return PartialView("_ContactList", employees);
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult UserManagement(int? jobTitleId = null, string teamIds = null)
        {
            teamIds = teamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));
            IList<LookupData> listJobTitle = commonDataService.GetLookupDataByCode(Constants.JobTitle);
            IList<SelectListItem> jobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = j.Id.ToString() == (jobTitleId != null ? jobTitleId.ToString() : string.Empty)
            }).ToList();

            var model = new UserManagementViewModel
            {
                JobTitleId = jobTitleId,
                TeamIds = teamIds,
                IsEnable = true,
                IsLocal = GetCategory(),
                JobTitles = jobTitles,
                CurrentPage = Constants.CurrentPage,
                ItemsPerPage = Constants.ItemsPerPage,
                IsShowDropdownCategory = userControlSession.HasPermissionOnFunction(FunctionKey.ManageAllEmployee, Policy.Read)
            };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult ListEmployee(UserManagementViewModel model)
        {
            IList<Employee> employees = new List<Employee>();
            if (model != null)
            {
                var criteria = new GetAllEmployeeCriteria
                {
                    Query = model.Keyword,
                    TeamIds = model.TeamIds,
                    EnableUser = model.IsEnable,
                    IsDeleteUser = model.IsDelete,
                    TitleId = model.JobTitleId,
                    IsLocalUser = GetCategory(model.IsLocal),
                    PageNumber = model.CurrentPage,
                    PageSize = model.ItemsPerPage
                };
                employees = employeeService.GetEmployees(criteria);

                var pagerOption = new PagerOptionModel
                {
                    CurrentPage = model.CurrentPage,
                    ItemsPerPage = model.ItemsPerPage,
                    TotalItem = (int)criteria.TotalRow
                };
                ViewBag.PagerOption = pagerOption;
            }

            ViewBag.HasPermissionToEdit = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
            ViewBag.EnableGroupDate = commonDataService.IsEnableGroupDate();

            return PartialView("_ListEmployee", employees);
        }

        public ActionResult Detail(int id)
        {
            Employee model = employeeService.GetEmployeeById(id);
            EmployeeShift employeeShift = employeeService.GetShiftByEmployeeId(id);

            ViewBag.Sun = employeeShift.SunShiftName;
            ViewBag.Mon = employeeShift.MonShiftName;
            ViewBag.Tue = employeeShift.TueShiftName;
            ViewBag.Wed = employeeShift.WedShiftName;
            ViewBag.Thu = employeeShift.ThuShiftName;
            ViewBag.Fri = employeeShift.FriShiftName;
            ViewBag.Sat = employeeShift.SatShiftName;
            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
            ViewBag.EnableGroupDate = commonDataService.IsEnableGroupDate();

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.CreateAccount)]
        public ActionResult Create()
        {
            return View(new CreateEmployeeViewModel());
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.CreateAccount)]
        public ActionResult Create(CreateEmployeeViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var param = new CreateEmployeeParam
                {
                    Code = model.Code,
                    DomainUsername = model.DomainUsername,
                    FullName = model.FullName,
                    Nickname = model.Nickname,
                    TeamId = model.TeamId
                };

                CommandResult commandResult = employeeService.Create(EmployeeSession.EmpId, param);
                if (commandResult.IsSuccessful)
                {
                    var token = cryptoService.GenerateRijndaelKeyIV();
                    string[] keyiv = token.Split(':');
                    var rijndaelKey = keyiv[0];
                    var rijndaelIV = keyiv[1];
                    var result = userService.Create(new CreateUserParam
                    {
                        RefId = (int?)commandResult.OutputId,
                        Code = param.Code,
                        Nickname = param.Nickname,
                        FullName = param.FullName,
                        BCryptPassword = cryptoService.Encrypt(param.Code, rijndaelKey, rijndaelIV),
                        Token = token
                    });

                    if (result.IsSuccessful)
                    {
                        employeeService.UpdateUserId(new UpdateUserIdCommand
                        {
                            EmpId = (int)commandResult.OutputId,
                            UserId = result.ObjectId,
                            ModifiedBy = EmployeeSession.EmpId
                        });
                        return RedirectToAction("Edit", new { id = commandResult.OutputId });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }

            return View(model);
        }

        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(int id)
        {
            Employee employee = employeeService.GetEmployeeById(id);
            if (employee != null)
            {
                if ((userControlSession.HasPermissionOnFunction(FunctionKey.ManageLocalEmployee, Policy.Host) && !employee.IsLocal)
                    && (userControlSession.HasPermissionOnFunction(FunctionKey.ManageExpatEmployee, Policy.Host) && employee.IsLocal))
                {
                    return RedirectToAction("AccessDenied", "Error");
                }

                employee.Coutries = commonDataService.GetAllCountry();
                IList<SupervisorNameModel> supervisors = GetSupervisors(id);
                employee.Verifier = GetVerifiers(supervisors);
                employee.Approver = GetApprovers(supervisors);
                employee.Disciplinarian = GetDisciplineSupervisors(supervisors);
                employee.EmpShiftList = GetShiftMonitors(supervisors);
                employee.ActingEmpList = GetActingSupervisors(supervisors);
                ViewBag.LengthOfServiceByGroupDate = employeeService.CaculateLengthOfService(employee.JoinedDate, employee.MilitaryFromDate, employee.MilitaryToDate);
                ViewBag.LengthOfServiceByCompanyDate = employeeService.CaculateLengthOfService(employee.CompanyDate, employee.MilitaryFromDate, employee.MilitaryToDate);
                ViewBag.EnableGroupDate = commonDataService.IsEnableGroupDate();

                ComposeViewBagsForEditEmployee();
                return View(employee);
            }
            else
            {
                return RedirectToAction("PageNotFound", "Error");
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(Employee model)
        {
            if (ModelState.IsValid)
            {
                EditEmployeeParam param = MappingEditEmployeeParam(model);
                CommandResult commandResult = employeeService.Edit(EmployeeSession.EmpId, param);
                if (commandResult.IsSuccessful)
                {
                    UpdateUserInfo(model.UserId, param);
                }
                else
                {
                    ViewBag.Updated = false;
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }

            model.Coutries = commonDataService.GetAllCountry();
            IList<SupervisorNameModel> supervisors = GetSupervisors(model.Id);
            model.Verifier = GetVerifiers(supervisors);
            model.Approver = GetApprovers(supervisors);
            model.Disciplinarian = GetDisciplineSupervisors(supervisors);
            model.EmpShiftList = GetShiftMonitors(supervisors);
            model.ActingEmpList = GetActingSupervisors(supervisors);
            ViewBag.EnableGroupDate = commonDataService.IsEnableGroupDate();

            ComposeViewBagsForEditEmployee();

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Delete(DeleteEmployeeParam param)
        {
            CommandResult commandResult = employeeService.Delete(EmployeeSession.EmpId, param);
            if (commandResult.IsSuccessful)
            {
                var result = userService.Delete(param.UserId);
                return Json(result);
            }

            return Json(commandResult);
        }

        [AllowAnonymous]
        public FileResult Avatar(string id)
        {
            var avatarName = string.IsNullOrEmpty(id) ? EmployeeSession.Code + ".jpg" : id + ".jpg";
            if (System.IO.File.Exists(Server.MapPath("/avatars/" + avatarName)))
            {
                return File("/avatars/" + avatarName, "image/jpeg");
            }

            return File("/avatars/avatar.jpg", "image/jpeg");
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public JsonResult UploadAvatar(string code, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                using (var stream = new MemoryStream())
                {
                    using (var processor = new ImageProcessor())
                    {
                        const int JpegCompression = 100;
                        ////var resizer = new ResizeFilter(80, 80, ResizeMethod.Crop, AnchorLocation.Middle);
                        ////processor.AddFilter(resizer);
                        processor.OutputFormat = OutputFormat.Jpeg;
                        processor.JpegCompression = JpegCompression;
                        processor.ProcessImage(file.InputStream, stream);
                        var image = Image.FromStream(stream);
                        image.Save(Server.MapPath("/avatars/") + code + ".jpg");
                        image.Dispose();

                        return Json(true);
                    }
                }
            }

            return Json(null);
        }

        [HttpGet]
        public ActionResult ManageSupervisor(int id, short roleId)
        {
            ViewBag.EmployeeId = id;
            ViewBag.RoleId = roleId;
            return View();
        }

        public CustomJsonResult GetEmployeeSupervisorByRoleId(int employeeId, short roleId)
        {
            IEnumerable<SupervisorModel> supervisors = employeeService.GetEmployeeSupervisorByRoleId(employeeId, roleId);
            return new CustomJsonResult { Data = supervisors, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public CustomJsonResult EditEmployeeSupervisor(int employeeId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
        {
            CommandResult commandResult = employeeService.EditEmployeeSupervisor(EmployeeSession.EmpId, employeeId, roleId, supervisors);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult ShiftManager(int id)
        {
            ViewBag.EmployeeId = id;
            ViewBag.RoleId = (short)RoleSystem.ShiftManager;

            return View();
        }

        public CustomJsonResult GetShiftManagerByEmployeeId(int employeeId)
        {
            IEnumerable<SupervisorModel> supervisors = employeeService.GetEmployeeSupervisorByRoleId(employeeId, (short)RoleSystem.ShiftManager);
            return new CustomJsonResult
            {
                Data = supervisors.Select(x => new Supervisor { OwnerId = x.OwnerId, Nickname = x.NickName, FullName = x.FullName }),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public void ToogleLeftNavigation(bool isShow)
        {
            userControlSession.Set<bool>(SessionKey.IsShowLeftNavigation, isShow);
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult DisciplineManager(int id)
        {
            ViewBag.EmployeeId = id;
            ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
            return View();
        }

        public CustomJsonResult GetDisciplineManagerByEmployeeId(int employeeId)
        {
            IEnumerable<Supervisor> supervisors = employeeService.GetDisciplineSettingByEmployeeId(employeeId);
            return new CustomJsonResult { Data = supervisors, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetMemberByManager(string keyword)
        {
            var criteria = new GetMemberByManagerCriteria
            {
                SupervisorId = EmployeeSession.EmpId,
                Query = keyword,
                ExcludedManager = false
            };
            IList<Employee> employees = employeeService.GetMemberByManager(criteria);
            const byte TotalItems = 10;

            return Json(employees.Take(TotalItems), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployees(
            string query,
            string teamIds = "",
            bool? enable = null,
            bool? isDelete = null,
            int? titleId = null,
            bool? isLocal = null)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = query,
                TeamIds = teamIds,
                EnableUser = enable,
                IsDeleteUser = isDelete,
                TitleId = titleId,
                IsLocalUser = isLocal
            };
            IList<Employee> employees = employeeService.GetEmployees(criteria);

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSupervisors(string roleIds, string query, string excludeIds)
        {
            var criteria = new GetEmployeeByRoleCriteria
            {
                ExcludeIds = excludeIds ?? string.Empty,
                Query = query.Trim(),
                RoleIds = roleIds
            };
            IList<Employee> employees = employeeService.GetEmployeesByRole(criteria);

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMemberByGender(string keyword, int gender)
        {
            var employees = employeeService.GetEmployeeByGender(keyword, gender);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateEmployeeCode()
        {
            string newCode = employeeService.GenerateEmployeeCode();
            return Json(newCode, JsonRequestBehavior.AllowGet);
        }

        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult ActivityLog(int id)
        {
            Employee employee = employeeService.GetEmployeeById(id);
            IEnumerable<LogObjectDetail> logs = employeeLogService.GetLogDetailById(id);
            ViewBag.Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName);
            ViewBag.EnableGroupDate = commonDataService.IsEnableGroupDate();

            return View("ActivityLog", logs);
        }

        private void UpdateUserInfo(int userId, EditEmployeeParam param)
        {
            var result = userService.UpdateInfo(new UpdateUserInfoParam
            {
                RefId = param.EmployeeId,
                UserId = userId,
                Code = param.Code,
                Nickname = param.Nickname,
                FullName = param.FullName,
                Email = param.Email,
                IsActive = param.Enabled
            });
            if (result.IsSuccessful)
            {
                result = userService.AssignRole(new AssignRoleParam
                {
                    UserId = userId,
                    RoleIds = param.Roles
                });

                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                    UpdateUserSession(param.EmployeeId);
                }
                else
                {
                    ViewBag.Updated = false;
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ViewBag.Updated = false;
                ModelState.AddModelError(string.Empty, result.Message);
            }
        }

        private void UpdateUserSession(int employeeId)
        {
            if (EmployeeSession.EmpId == employeeId)
            {
                userControlSession.Set<EmployeeSession>(SessionKey.Employee, authService.GetByUserId(userControlSession.UserId));
            }
        }

        private IList<SupervisorNameModel> GetSupervisors(int empId)
        {
            var criteria = new GetListSupervisorNameCriteria
            {
                EmpId = empId,
                Role = null,
                RequestType = null
            };
            IList<SupervisorNameModel> supervisors = employeeService.GetListSupervisorName(criteria);
            return supervisors;
        }

        private static string GetSupervisors(IList<SupervisorNameModel> supervisors, short roleId)
        {
            string supervisorsName = string.Empty;
            IList<SupervisorNameModel> supervisorsFilter;
            const byte MaxDisplayItems = 3;

            if (supervisors != null)
            {
                supervisorsFilter = supervisors.Where(x => x.ActionType == roleId).Take(MaxDisplayItems).ToList();
                supervisorsName = string.Join(", ", supervisorsFilter.Select(x => x.NickName));

                if (supervisorsFilter.Count > MaxDisplayItems)
                {
                    supervisorsName += ",...";
                }
            }

            return supervisorsName;
        }

        private static string GetVerifiers(IList<SupervisorNameModel> supervisors)
        {
            return GetSupervisors(supervisors, (short)RoleSystem.Verifier);
        }

        private static string GetApprovers(IList<SupervisorNameModel> supervisors)
        {
            return GetSupervisors(supervisors, (short)RoleSystem.Approver);
        }

        private static string GetShiftMonitors(IList<SupervisorNameModel> supervisors)
        {
            return GetSupervisors(supervisors, (short)RoleSystem.ShiftManager);
        }

        private static string GetActingSupervisors(IList<SupervisorNameModel> supervisors)
        {
            return GetSupervisors(supervisors, (short)RoleSystem.OnBehalfRequest);
        }

        private static string GetDisciplineSupervisors(IList<SupervisorNameModel> supervisors)
        {
            return GetSupervisors(supervisors, (short)RoleSystem.DisciplineManager);
        }

        private void ComposeViewBagsForEditEmployee()
        {
            var jobLevels = jobLevelService.GetJobLevel();
            ViewBag.JobTitles = commonDataService.GetLookupDataByCode(Constants.JobTitle);
            ViewBag.JobLevel = jobLevels;
            ViewBag.Grades = jobLevels?.Where(jl => jl.Name.StartsWith("E")).ToList();
            ViewBag.Marriages = Utilities.Marriages;
            ViewBag.Roles = roleService.GetBySiteId(userControlSession.SiteId);
        }

        private EditEmployeeParam MappingEditEmployeeParam(Employee model)
        {
            return new EditEmployeeParam
            {
                EmployeeId = model.Id,
                Code = model.Code.Trim(),
                DomainUsername = model.DomainUsername,
                FullName = model.FullName,
                Nickname = model.NickName,
                EmployeeCard = model.EmployeeCard,
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                JobTitleId = model.JobTitleId,
                JobLevelId = model.JobLevelId,
                ExpertGradeId = model.ExpertGradeId,
                Enabled = model.Enabled,
                Phone = model.Phone,
                Gender = model.Gender,
                Roles = BuildRole(model.Roles),
                JoinedDate = model.JoinedDate,
                CompanyDate = model.CompanyDate,
                ConfirmedDate = model.ConfirmedDate,
                ResignedDate = model.ResignedDate,
                ResignedReason = model.ResignedReason,
                Skype = model.Skype,
                LetsTalk = model.LetsTalk,
                Marriage = model.Marriage,
                PermanentAddress = model.Address,
                CurrentAddress = model.CurrentAddress,
                IdCardNumber = model.CardNo,
                PassportNo = model.PassportNo,
                PassportExp = model.PassportExp,
                TeamId = model.Team.Id,
                IsLocal = model.IsLocal,
                CountryCode = model.CountryCode,
                CanOT = model.CanOT,
                SpecialOT = model.SpecialOT,
                WorkingDays = model.WorkingDays,
                WorkingHours = model.WorkingHours,
                WorkingHoursbyWeek = model.WorkingHoursbyWeek,
                IsTopManager = model.IsTopManager,
                OffDaysbyWeek = model.OffDaysbyWeek,
                PlaceOfBirth = model.PlaceOfBirth,
                Ethnic = model.Ethnic,
                Native = model.Native,
                Religion = model.Religion,
                DateOfIdCard = model.DateOfIdCard,
                PlaceOfIdCard = model.PlaceOfIdCard,
                HomePhone = model.HomePhone,
                Qualification = model.Qualification,
                Major = model.Major,
                IsExpatUseHoliday = !model.IsLocal && model.IsExpatUseHoliday,
                PickupRemark = model.PickupRemark,
                PromotionDate = model.PromotionDate,
                TeamDate = model.TeamDate,
                MilitaryFromDate = model.MilitaryFromDate,
                MilitaryToDate = model.MilitaryToDate
            };
        }

        private string BuildRole(IList<Role> roles)
        {
            IList<Role> systemRoles = roleService.GetBySiteId(userControlSession.SiteId);
            var defaultRoles = systemRoles.Where(x => x.IsDefault).Select(x => x.RoleId);

            if (roles != null && roles.Count > 0)
            {
                var result = string.Join(",", roles.Select(i => i.RoleId));
                return roles.Any(x => defaultRoles.Any(y => x.RoleId == y)) ? result : string.Join(",", defaultRoles) + "," + result;
            }
            else
            {
                return string.Join(",", defaultRoles);
            }
        }

        private bool? GetCategory(bool? isLocal = null)
        {
            bool? category = isLocal;
            if (!userControlSession.HasPermissionOnFunction(FunctionKey.ManageAllEmployee, Policy.Read))
            {
                if (userControlSession.HasPermissionOnFunction(FunctionKey.ManageLocalEmployee, Policy.Read))
                {
                    category = true;
                }
                else
                {
                    if (userControlSession.HasPermissionOnFunction(FunctionKey.ManageExpatEmployee, Policy.Read))
                    {
                        category = false;
                    }
                }
            }

            return category;
        }
    }
}