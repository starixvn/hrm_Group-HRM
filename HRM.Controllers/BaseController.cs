namespace HRM.Controllers
{
    using GO.UserControl.Client.Core.Session;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.Core.Models;
    using Microsoft.AspNetCore.Mvc;

    [HrmAuthorize]
    public class BaseController : Controller
    {
        private readonly IUserControlSession userControlSession;

        public BaseController(IUserControlSession userControlSession)
        {
            this.userControlSession = userControlSession;
        }

        public EmployeeSession EmployeeSession => userControlSession.Get<EmployeeSession>(SessionKey.Employee);
    }
}