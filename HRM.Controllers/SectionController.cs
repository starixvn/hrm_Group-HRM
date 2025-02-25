using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Models;
    using Core.Services;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.Criteria.Section;
    using HRM.Models;
    using HRM.Services;
    using HRM.Services.Interfaces;
    using HRM.ViewModels;
    using HRM.ViewModels.Section;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class SectionController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly ICommonDataService commonDataService;
        private readonly IUnitService unitService;
        private readonly ISectionService sectionService;
        private readonly IBaseService baseService;
        private readonly IDepartmentService departmentService;

        public SectionController(
            IUnitService unitService,
            IUserControlSession userControlSession,
            ICommonDataService commonDataService,
            IDepartmentService departmentService,
            ISectionService sectionService,
            IBaseService baseService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.commonDataService = commonDataService;
            this.sectionService = sectionService;
            this.baseService = baseService;
            this.departmentService = departmentService;
            this.unitService = unitService;
        }

        [HrmPermission(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult Index()
        {
            var hasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.SectionManagement, Policy.Host);

            return View(hasPermission);
        }

        [HttpPost]
        [HrmPermission(Policy.Read, FunctionKey.SectionManagement)]
        public ActionResult GetSections(bool? isActive = null)
        {
            var sections = sectionService.GetSection(isActive);
            return new CustomJsonResult { Data = sections, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult GetDeparments()
        {
            var departments = departmentService.GetDepartment();
            return new CustomJsonResult { Data = departments, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
        public JsonResult Create(AddSectionViewModel model)
        {
            var commandResult = new CommandResult();
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertSectionCriteria
                {
                    Code = model.Code,
                    Name = model.Name,
                    IsActive = model.IsActive,
                    DepartmentId = model.DepartmentId,
                    ModifiedBy = EmployeeSession.EmpId
                };
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult GetById(int id)
        {
            Section section = sectionService.GetById(id);
            IList<Department> departments = departmentService.GetDepartment();
            var model = new EditSectionViewModel
            {
                SectionId = id,
                Code = section.Code,
                Name = section.Name,
                IsActive = section.IsActive,
                DepartmentId = section.Department != null ? section.Department.Id : 0,
                Departments = departments
            };
            return new CustomJsonResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
        public ActionResult Edit(int id)
        {
            return View(id);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
        public JsonResult Edit(EditSectionViewModel model)
        {
            var commandResult = new CommandResult();
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateSectionCriteria
                {
                    SectionId = model.SectionId,
                    Code = model.Code,
                    Name = model.Name,
                    IsActive = model.IsActive,
                    DepartmentId = model.DepartmentId,
                    ModifiedBy = EmployeeSession.EmpId
                };
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
        public CustomJsonResult Delete(int id)
        {
            CommandResult commandResult = sectionService.Delete(EmployeeSession.EmpId, id);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public CustomJsonResult GetOrgChart()
        {
            IList<Unit> units = unitService.GetUnit();
            IList<Department> department = departmentService.GetDepartment();
            IList<Team> teams = commonDataService.GetAllTeam();
            IList<Section> sections = sectionService.GetSection();

            IEnumerable<MultiSelectViewModel> orgchart = GetOrgChart(units, department, teams, sections);

            return new CustomJsonResult { Data = orgchart, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private IEnumerable<MultiSelectViewModel> GetOrgChart(
            IList<Unit> units,
            IList<Department> department,
            IList<Team> teams,
            IList<Section> sections)
        {
            var orgCharts = units.Select(u => new MultiSelectViewModel
            {
                Id = u.Id,
                Text = u.Name,
                Items = department.Where(d => d.Unit != null && d.Unit.Id == u.Id).Select(d => new MultiSelectViewModel
                {
                    Id = d.Id,
                    Text = d.Name,
                    ParentId = u.Id,
                    Items = sections.Where(s => s.Department != null && s.Department.Id == d.Id).Select(s => new MultiSelectViewModel
                    {
                        Id = s.Id,
                        Text = s.Name,
                        ParentId = d.Id,
                        Items = teams.Where(t => t.Section != null && t.Section.Id == s.Id).Select(t => new MultiSelectViewModel
                        {
                            Id = t.Id,
                            Text = t.Name,
                            IsLeaf = true,
                            ParentId = s.Id,
                        }).ToList()
                    }).ToList()
                }).ToList()
            });

            return orgCharts;
        }
    }
}