using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Filters;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.Employee;
    using HRM.Criteria.Group;
    using HRM.Models;
    using HRM.Models.Group;
    using HRM.Models.Supervisor;
    using HRM.Resources;
    using HRM.Services;
    using HRM.ViewModels.EmployeeGroup;
    using HRM.ViewModels.Team;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    [HrmPermission(Policy.Read, FunctionKey.TeamManagement)]
    public class TeamController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly ITeamService teamService;
        private readonly ISectionService sectionService;
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;

        public TeamController(
            IUserControlSession userControlSession,
            ITeamService teamService,
            ISectionService sectionService,
            IBaseService baseService,
            IEmployeeService employeeService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.teamService = teamService;
            this.sectionService = sectionService;
            this.baseService = baseService;
            this.employeeService = employeeService;
        }

        public ActionResult Index(string keyword = null)
        {
            IList<Team> teams = teamService.GetAll(keyword: keyword);
            ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.TeamManagement, Policy.Host);
            ViewBag.HasPermissionToViewEmployee = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Read);
            return View(teams);
        }

        [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
        public ActionResult Create()
        {
            var model = new TeamViewModel
            {
                Sections = sectionService.GetSection(),
                Employees = GetAllEmployees()
            };
            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
        public ActionResult Create(TeamViewModel model)
        {
            model.Sections = sectionService.GetSection();
            model.Employees = GetAllEmployees();
            if (ModelState.IsValid)
            {
                var criteria = new CreateTeamCriteria
                {
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    SectionId = model.SectionId ?? 0,
                    HeadId = model.HeadId ?? 0,
                    ManagerIds = model.ManagerIds,
                    LeaderIds = model.LeaderIds,
                    SupervisorIds = model.SupervisorIds,
                    MailGroup = model.MailGroup,
                    CreatedBy = EmployeeSession.EmpId,
                    LocalBasicAnnualLeaveBalance = model.LocalBasicAnnualLeaveBalance ?? 0,
                    LocalWorkingDays = model.LocalWorkingDays ?? 0,
                    LocalWorkingHours = model.LocalWorkingHours ?? 0,
                    LocalOffDaysbyWeek = model.LocalOffDaysbyWeek ?? 0,
                    LocalWorkingHoursbyWeek = model.LocalWorkingHoursbyWeek,
                    ExpatBasicAnnualLeaveBalance = model.ExpatBasicAnnualLeaveBalance ?? 0,
                    ExpatWorkingDays = model.ExpatWorkingDays ?? 0,
                    ExpatWorkingHours = model.ExpatWorkingHours ?? 0,
                    ExpatOffDaysbyWeek = model.ExpatOffDaysbyWeek ?? 0,
                    ExpatWorkingHoursbyWeek = model.ExpatWorkingHoursbyWeek
                };
                CommandResult result = baseService.ExecuteNonQuery(criteria);

                if (result.IsSuccessful)
                {
                    return RedirectToAction("Index", "Team");
                }

                ModelState.AddModelError(string.Empty, result.Message);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return View(model);
        }

        [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
        public ActionResult Edit(int id)
        {
            Team team = teamService.GetById(id);
            if (team == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var unitHead = team.Managers.FirstOrDefault(x => x.RoleId == (byte)TeamManagerRole.Head);
            var model = new TeamViewModel
            {
                TeamCode = team.Code,
                TeamName = team.Name,
                MailGroup = team.MailGroup,
                LocalBasicAnnualLeaveBalance = team.LocalBasicAnnualLeaveBalance ?? 0,
                LocalWorkingDays = team.LocalWorkingDays ?? 0,
                LocalWorkingHours = team.LocalWorkingHours ?? 0,
                LocalOffDaysbyWeek = team.LocalOffDaysbyWeek ?? 0,
                LocalWorkingHoursbyWeek = team.LocalWorkingHoursbyWeek,
                ExpatBasicAnnualLeaveBalance = team.ExpatBasicAnnualLeaveBalance ?? 0,
                ExpatWorkingDays = team.ExpatWorkingDays ?? 0,
                ExpatWorkingHours = team.ExpatWorkingHours ?? 0,
                ExpatOffDaysbyWeek = team.ExpatOffDaysbyWeek ?? 0,
                ExpatWorkingHoursbyWeek = team.ExpatWorkingHoursbyWeek,
                HeadId = unitHead?.EmpId,
                UnitHead = unitHead?.DisplayName,
                ManagerIds = string.Join(",", team.Managers.Where(x => x.RoleId == (byte)TeamManagerRole.Manager).Select(e => e.EmpId)),
                SupervisorIds = string.Join(",", team.Managers.Where(x => x.RoleId == (byte)TeamManagerRole.Supervisor).Select(e => e.EmpId)),
                LeaderIds = string.Join(",", team.Managers.Where(x => x.RoleId == (byte)TeamManagerRole.Leader).Select(e => e.EmpId)),
                SectionId = team.Section != null ? team.Section.Id : 0,
                Sections = sectionService.GetSection(),
                Employees = GetAllEmployees()
            };

            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
        public ActionResult Edit(TeamViewModel model)
        {
            if (model != null)
            {
                model.Sections = sectionService.GetSection();
                model.Employees = GetAllEmployees();
                if (ModelState.IsValid)
                {
                    var criteria = new EditTeamCriteria
                    {
                        TeamId = model.Id,
                        Code = model.TeamCode,
                        Name = model.TeamName,
                        SectionId = model.SectionId ?? 0,
                        MailGroup = model.MailGroup,
                        HeadId = model.HeadId ?? 0,
                        ManagerIds = model.ManagerIds,
                        LeaderIds = model.LeaderIds,
                        SupervisorIds = model.SupervisorIds,
                        ModifiedBy = EmployeeSession.EmpId,
                        LocalBasicAnnualLeaveBalance = model.LocalBasicAnnualLeaveBalance ?? 0,
                        LocalWorkingDays = model.LocalWorkingDays ?? 0,
                        LocalWorkingHours = model.LocalWorkingHours ?? 0,
                        LocalOffDaysbyWeek = model.LocalOffDaysbyWeek ?? 0,
                        LocalWorkingHoursbyWeek = model.LocalWorkingHoursbyWeek,
                        ExpatBasicAnnualLeaveBalance = model.ExpatBasicAnnualLeaveBalance ?? 0,
                        ExpatWorkingDays = model.ExpatWorkingDays ?? 0,
                        ExpatWorkingHours = model.ExpatWorkingHours ?? 0,
                        ExpatOffDaysbyWeek = model.ExpatOffDaysbyWeek ?? 0,
                        ExpatWorkingHoursbyWeek = model.ExpatWorkingHoursbyWeek
                    };
                    CommandResult result = baseService.ExecuteNonQuery(criteria);

                    if (result.IsSuccessful)
                    {
                        ViewBag.Updated = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
                }
            }

            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
        public CustomJsonResult Delete(int id)
        {
            CommandResult commandResult = teamService.Delete(EmployeeSession.EmpId, id);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult TeamSupervisor(int id)
        {
            IEnumerable<TeamSupervisorModel> supervisors = teamService.GetSupervisorByTeamId(id);
            ViewBag.TeamId = id;
            ViewBag.TeamCode = supervisors.Any() ? supervisors.First().Code : string.Empty;

            return View(supervisors);
        }

        [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
        public ActionResult ManageSupervisor(int id, short roleId)
        {
            ViewBag.TeamId = id;
            ViewBag.RoleId = roleId;

            return View();
        }

        [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
        public CustomJsonResult GetTeamSupervisorByRoleId(int teamId, short roleId)
        {
            IEnumerable<SupervisorModel> supervisors = teamService.GetTeamSupervisorByRoleId(teamId, roleId);

            return new CustomJsonResult { Data = supervisors, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
        public ActionResult ShiftManager(int id)
        {
            ViewBag.TeamId = id;
            ViewBag.RoleId = (short)RoleSystem.ShiftManager;

            return View();
        }

        [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
        public CustomJsonResult GetShiftManagerByTeamId(int teamId)
        {
            IEnumerable<SupervisorModel> supervisors = teamService.GetTeamSupervisorByRoleId(teamId, (short)RoleSystem.ShiftManager);

            return new CustomJsonResult
            {
                Data = supervisors.Select(x => new Supervisor { OwnerId = x.OwnerId, Nickname = x.NickName, FullName = x.FullName }),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
        public ActionResult DisciplineManager(int id)
        {
            ViewBag.TeamId = id;
            ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
            return View();
        }

        [HttpGet]
        public CustomJsonResult GetDisciplineManagerByTeamId(int teamId)
        {
            IEnumerable<Supervisor> supervisors = teamService.GetDisciplineManagerByTeamId(teamId);
            return new CustomJsonResult { Data = supervisors, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
        public CustomJsonResult EditTeamSupervisor(int teamId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
        {
            CommandResult commandResult = teamService.EditTeamSupervisor(EmployeeSession.EmpId, teamId, roleId, supervisors);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult DisciplineRule(int id)
        {
            Team team = teamService.GetById(id);
            if (team == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var model = new DisciplineRuleViewModel
            {
                TeamId = team.Id,
                Team = team.Name,
                IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.TeamManagement, Policy.Host),
                DisciplineRules = teamService.GetDisciplineRuleByTeam(id)
            };
            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
        public CustomJsonResult EditDisciplineRule(int teamId, IEnumerable<DisciplineRule> disciplineRules)
        {
            CommandResult commandResult = teamService.EditDisciplineRule(EmployeeSession.EmpId, teamId, disciplineRules);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IList<Employee> GetAllEmployees()
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = null,
                TeamIds = null,
                EnableUser = true,
                IsDeleteUser = null,
                TitleId = null,
                IsLocalUser = null
            };
            IList<Employee> employees = employeeService.GetEmployees(criteria).ToList();

            return employees;
        }
    }
}