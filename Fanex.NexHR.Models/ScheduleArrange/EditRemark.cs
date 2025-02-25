namespace Fanex.NexHR.Models.ScheduleArrange
{
    using System.Web.Mvc;

    public class EditRemark
    {
        public int EmployeeId { get; set; }

        [AllowHtml]
        public string Remark { get; set; }
    }
}