namespace HRM.Site
{
    using System.Web.Routing;

    public static class RouteConfig
    {
        private const string DefaultActionName = "index";

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "ManageTeam",
               url: "Employee/AddTeam/{teamId}",
               defaults: new { controller = "employee", action = "AddTeam", teamId = UrlParameter.Optional });

            routes.MapRoute(
               name: "MonthlyWorkingHours",
               url: "Report/MonthlyWorkingHours",
               defaults: new { controller = "MonthlyWorkingHours", action = DefaultActionName });

            routes.MapRoute(
               name: "WeeklyWorkingHours",
               url: "Report/WeeklyWorkingHours",
               defaults: new { controller = "WeeklyWorkingHours", action = DefaultActionName });

            routes.MapRoute(
               name: "AttendanceReport",
               url: "Report/Attendance",
               defaults: new { controller = "AttendanceReport", action = DefaultActionName });

            routes.MapRoute(
               name: "AttendanceDetailsReport",
               url: "Report/AttendanceDetails",
               defaults: new { controller = "AttendanceReport", action = "Details" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "home", action = DefaultActionName, id = UrlParameter.Optional });
        }
    }
}