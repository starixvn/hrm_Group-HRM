namespace Fanex.NexHR.ViewModels.Request
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Request.ChangeShift;

    public class ProcessRequestViewModel
    {
        public long RequestId { get; set; }
        public byte RoleId { get; set; }
        public int EmployeeId { get; set; }
        public RequestCommand Command { get; set; }
        public byte RequestTypeId { get; set; }
        public RequestViewType ViewType { get; set; }
        [Display(Name = "Remark")]
        public string Comment { get; set; }

        public RequestEntity CheckInOutDetail { get; set; }
        public RequestEntity LateInEarlyOutDetail { get; set; }
        public IList<ChangeShiftDetail> ChangeShiftDetails { get; set; }
        public IList<LeaveDisciplineModel> LeaveDisciplines { get; set; }
        public string IgnoredDisciplineDetails { get; set; }

        public IList<IgnoredDisciplineModel> IgnoredDisciplines
        {
            get
            {
                if (IgnoredDisciplineDetails != null)
                {
                    return JsonConverter.FromJsonArray<IgnoredDisciplineModel>(IgnoredDisciplineDetails);
                }

                return new List<IgnoredDisciplineModel>();
            }
        }
    }
}