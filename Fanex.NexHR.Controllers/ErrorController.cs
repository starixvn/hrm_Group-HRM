namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;

    public class ErrorController : Controller
    {
        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult InternalError()
        {
            return View();
        }
    }
}