namespace Fanex.NexHR.Site
{
    using System;
    using System.Web.Security;

    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            else
            {
                Response.Redirect("~/home/index");
            }
        }
    }
}