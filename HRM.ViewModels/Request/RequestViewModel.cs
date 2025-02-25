namespace HRM.ViewModels.Request
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Helpers;
    using HRM.Models;
    using HRM.Models.Request;
    using HRM.Resources;

    public class RequestViewModel
    {
        private bool isFilterByAffectedDate;

        public int DateOption { get; set; }

        [Display(Name = "Status")]
        public int StatusId { get; set; } = Constants.DefaultRequestFilterStatusId;

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; } = DateTimeOffset.Now.Date;

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; } = DateTimeOffset.Now.Date;

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
                string statusIds;
                switch (StatusId)
                {
                    case Constants.PendingAndVerifiedStatusId:
                        statusIds = "1,2";
                        break;

                    case 0:
                        statusIds = string.Empty;
                        break;

                    default:
                        statusIds = StatusId.ToString();
                        break;
                }

                return statusIds;
            }
        }

        public string ActionName
        {
            get
            {
                var action = string.Empty;
                switch (ViewType)
                {
                    case RequestViewType.PersonalSpace:
                        action = "Member";
                        break;

                    case RequestViewType.HRSpace:
                        action = "HRView";
                        break;

                    case RequestViewType.CompanySpace:
                        action = "DailyView";
                        break;

                    case RequestViewType.SASpace:
                        action = "VoidRequest";
                        break;

                    default:
                        action = string.Empty;
                        break;
                }

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

            set
            {
                isFilterByAffectedDate = value;
            }
        }

        public IList<RequestEntity> Requests { get; set; }

        public string BreadcrumbText => EnumHelper.GetDescription(ViewType);

        public string PageTitle
        {
            get
            {
                var pageTitle = string.Empty;
                switch (ViewType)
                {
                    case RequestViewType.PersonalSpace:
                        pageTitle = Labels.MemberRequest;
                        break;

                    case RequestViewType.HRSpace:
                        pageTitle = Labels.EmployeeRequest;
                        break;

                    case RequestViewType.CompanySpace:
                        pageTitle = Labels.DailyRequests;
                        break;

                    case RequestViewType.SASpace:
                        pageTitle = Labels.VoidRequest;
                        break;

                    default:
                        pageTitle = Labels.NA;
                        break;
                }

                return pageTitle;
            }
        }
    }
}