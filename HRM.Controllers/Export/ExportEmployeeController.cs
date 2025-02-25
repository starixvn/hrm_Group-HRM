using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System.Collections.Generic;
    using HRM.Criteria.Employee;
    using HRM.Models;
    using HRM.Models.EmployeeInformation;
    using HRM.Services;
    using HRM.ViewModels;
    using GO.UserControl.Client.Core.Session;

    public class ExportEmployeeController : BaseExportController
    {
        private readonly IEmployeeService employeeService;
        private readonly IExportExcelEmployeeInfoService excelService;

        public ExportEmployeeController(
            IUserControlSession userControlSession,
            IEmployeeService employeeService,
            IExportExcelEmployeeInfoService excelService) : base(userControlSession)
        {
            this.employeeService = employeeService;
            this.excelService = excelService;
        }

        public ActionResult EmployeeBasicInfo(UserManagementViewModel model)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = model.Keyword,
                TeamIds = model.TeamIds,
                EnableUser = model.IsEnable,
                IsDeleteUser = model.IsDelete,
                TitleId = model.JobTitleId,
                IsLocalUser = model.IsLocal
            };

            IList<Employee> employees = employeeService.GetEmployees(criteria);
            excelService.ExportEmployeeBasicInfo(employees);

            return new EmptyResult();
        }

        public ActionResult EmployeeDetailsInfo(UserManagementViewModel model)
        {
            var criteria = new GetEmployeeFullProfileCriteria
            {
                Query = model.Keyword,
                TeamIds = model.TeamIds,
                EnableUser = model.IsEnable,
                IsDeleteUser = model.IsDelete,
                TitleId = model.JobTitleId,
                IsLocalUser = model.IsLocal
            };

            IList<EmployeeFullProfile> profiles = employeeService.GetEmployeeFullProfile(criteria);
            excelService.ExportEmployeeFullProfile(profiles);

            return new EmptyResult();
        }
    }
}