using HRM.Core;

namespace HRM.ViewModels.Request;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Constants;
using Core.Enums;
using Core.Helpers;
using Models;
using HRM.Models.Request;
using Resources;

public class RequestViewModel
{
    private bool isFilterByAffectedDate;

    public int DateOption { get; set; }

    [Display(Name = "Status")]
    public int StatusId { get; set; } = Constants.DefaultRequestFilterStatusId;

    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
    public DateTime FromDate { get; set; } = DateTimeOffset.Now.Date.ToClientTimeZone();

    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
    public DateTime ToDate { get; set; } = DateTimeOffset.Now.Date.ToClientTimeZone();

    public int EmployeeId { get; set; } = -1;

    public string Employee { get; set; }

    public string TeamIds { get; set; }

    public string Team { get; set; }

    public int RequestType { get; set; } = (int)RequestFilter.All;

    public int? LeaveType { get; set; }

    public IList<LeaveType> LeaveTypes { get; set; }

    public byte? IsLocal { get; set; }

    public int CurrentUserId { get; set; }

    public RequestViewType ViewType { get; set; }

    public string StatusIds
    {
        get
        {
            var statusIds = StatusId switch
            {
                Constants.PendingAndVerifiedStatusId => "1,2",
                0 => string.Empty,
                _ => StatusId.ToString()
            };

            return statusIds;
        }
    }

    public string ActionName
    {
        get
        {
            var action = ViewType switch
            {
                RequestViewType.PersonalSpace => "Member",
                RequestViewType.HRSpace => "HRView",
                RequestViewType.CompanySpace => "DailyView",
                RequestViewType.SASpace => "VoidRequest",
                _ => string.Empty
            };

            return action;
        }
    }

    public bool IsAllowChangeStatus => ViewType != RequestViewType.CompanySpace;

    public bool IsFilterByAffectedDate
    {
        get
        {
            if (StatusId == Constants.DefaultRequestFilterStatusId)
            {
                isFilterByAffectedDate = ViewType != RequestViewType.PersonalSpace;
            }

            return isFilterByAffectedDate;
        }

        set => isFilterByAffectedDate = value;
    }

    public IList<RequestEntity> Requests { get; set; }

    public string BreadcrumbText => EnumHelper.GetStringValue(ViewType);

    public string PageTitle
    {
        get
        {
            var pageTitle = ViewType switch
            {
                RequestViewType.PersonalSpace => Labels.MemberRequest,
                RequestViewType.HRSpace => Labels.EmployeeRequest,
                RequestViewType.CompanySpace => Labels.DailyRequests,
                RequestViewType.SASpace => Labels.VoidRequest,
                _ => Labels.NA
            };

            return pageTitle;
        }
    }
}