namespace Fanex.NexHR.Controllers
{
    using System.Configuration;
    using System.Web.Mvc;
    using GO.UserControl.Client.Core.Session;
    using Resources;
    using Services;

    public class EmailController : BaseController
    {
        private readonly IEmailService emailService;

        public EmailController(
            IUserControlSession userControlSession,
            IEmailService emailService) : base(userControlSession)
        {
            this.emailService = emailService;
        }

        [HttpGet]
        public JsonResult Index()
        {
            string emailSD = ConfigurationManager.AppSettings["EmailSD"];
            emailService.SendEmail(
                emailSD,
                receiverEmail: emailSD,
                mailCC: string.Empty, subject: Labels.TestSendMail,
                body: Labels.TestSendMail);

            return Json("Done", JsonRequestBehavior.AllowGet);
        }
    }
}