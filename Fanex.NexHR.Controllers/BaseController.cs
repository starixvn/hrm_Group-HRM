namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using GO.UserControl.Client.Core.Session;

    [HRMAuthorize]
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