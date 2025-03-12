namespace HRM.ViewModels.Request;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Enums;
using HRM.Models.Discipline;
using HRM.Models.Request;
using HRM.Models.Request.ChangeShift;
using Newtonsoft.Json;

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

    public IList<IgnoredDisciplineModel> IgnoredDisciplines =>
        IgnoredDisciplineDetails != null
            ? JsonConvert.DeserializeObject<IList<IgnoredDisciplineModel>>(IgnoredDisciplineDetails)
            : new List<IgnoredDisciplineModel>();
}