using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{

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