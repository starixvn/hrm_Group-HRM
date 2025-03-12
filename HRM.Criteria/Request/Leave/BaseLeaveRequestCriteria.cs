namespace HRM.Criteria.Request.Leave;

using System;
using System.Collections.Generic;
using System.Text;
using Fanex.Data;
using Fanex.Data.Repository;
using Core.Constants;
using Core.Enums;
using HRM.Models.Request;

public abstract class BaseLeaveRequestCriteria : CriteriaBase
{
    private const string LeaveRequestDetailTpl =
        "<Row RequestId='{0}' EmpId='{1}' OnDate='{2}' "
        + "LeaveType='{3}' IsHalfDay='{4}' WorkType='{5}' FromTime='{6}' ToTime='{7}'></Row>";

    public long RequestId { get; set; }

    [SpParam(SqlParamName = "EmpId")]
    public int EmployeeId { get; set; }

    [SpParam(Ignored = true)]
    public string EmployeeName { get; set; }

    public string Remark { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public bool IsHospitalApplication { get; set; }

    public int LeaveTypeId { get; set; }

    [SpParam(Ignored = true)]
    public string LeaveTypeName { get; set; }

    public decimal TotalDays { get; set; }

    public int? VerifierId { get; set; }

    public int? ApproverId { get; set; }

    public string RefNo { get; set; }

    [SpParam(Ignored = true)]
    public IList<LeaveRequestDetail> RequestDetail { get; set; }

    public string ListDetail
    {
        get
        {
            var value = new StringBuilder(Constants.BeginRootTag);

            if (RequestDetail != null)
            {
                foreach (var detail in RequestDetail)
                {
                    value.AppendFormat(
                        LeaveRequestDetailTpl,
                        0,
                        EmployeeId,
                        detail.OnDate,
                        detail.WorkType == (int)WorkType.Leave ? detail.LeaveTypeId.ToString() : string.Empty,
                        detail.IsHalfDay,
                        detail.WorkType,
                        detail.FromTime,
                        detail.ToTime);
                }
            }

            return value.Append(Constants.EndRootTag).ToString();
        }
    }

    public abstract override string GetSettingKey();

    public abstract override bool IsValid();
}