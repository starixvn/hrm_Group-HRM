namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Helpers;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.API.Request;
    using HRM.Criteria.Auth;
    using HRM.Models.API.Request;
    using HRM.Models.API.Todolist;
    using HRM.Repositories;
    using HRM.Resources;

    public class APITodolistRequestService : IAPITodolistRequestService
    {
        private readonly IAPITodolistRequestRepository requestRepository;
        private readonly IBaseService baseService;
        private readonly IRequestService requestService;

        public APITodolistRequestService(
            IAPITodolistRequestRepository requestRepository,
            IBaseService baseService,
            IRequestService requestService)
        {
            this.requestRepository = requestRepository;
            this.baseService = baseService;
            this.requestService = requestService;
        }

        public IList<TodolistItem> GetPendingRequestByUserId(int userId)
        {
            var employee = GetEmployeeByUserId(userId);
            var criteria = new GetPendingRequestByUserIdCriteria
            {
                UserId = userId
            };
            IList<RequestEntity> requests = requestRepository.GetPendingRequestByUserId(criteria);

            var items = new List<TodolistItem>();
            foreach (var request in requests)
            {
                switch (request.RequestTypeId)
                {
                    case (byte)RequestType.Leave:
                        items.Add(GetLeaveRequest(employee.EmpId, request));
                        break;

                    case (byte)RequestType.ChangeShift:
                        items.Add(GetChangeShiftRequest(employee.EmpId, request));
                        break;

                    case (byte)RequestType.CheckIn:
                    case (byte)RequestType.CheckOut:
                        items.Add(GetCheckInOutRequest(employee.EmpId, request));
                        break;

                    case (byte)RequestType.LateIn:
                        items.Add(GetLateInRequest(employee.EmpId, request));
                        break;

                    case (byte)RequestType.EarlyOut:
                        items.Add(GetEarlyOutRequest(employee.EmpId, request));
                        break;

                    case (byte)RequestType.KeepInView:
                        items.Add(GetKeepInViewRequest(employee.EmpId, request));
                        break;

                    default:
                        // nothing
                        break;
                }
            }

            return items;
        }

        private EmployeeSession GetEmployeeByUserId(int userId)
        {
            var criteria = new GetEmployeeLoginByUserIdCriteria
            {
                UserId = userId
            };
            return baseService.GetDataEntity<EmployeeSession>(criteria);
        }

        private TodolistItem GetLeaveRequest(int actorId, RequestEntity request)
        {
            var leaveDateTime = GetDateTimeForLeaveRequest(request);
            var leaveDayDetail = GetLeaveDayDetail(request);
            var name = new StringBuilder();
            name.AppendFormat("<div>You have a Leave Request (Type: {0}) of <strong>{1} ({2})</strong> {3}</div>",
                request.LeaveTypeName,
                request.FullName,
                request.Nickname,
                string.Format("<span class='request leave'>{0}</span> {1}", leaveDateTime, leaveDayDetail));
            AppendStatus(name, request.StatusId);
            AppendRemark(name, request.Remark);

            var item = new TodolistItem
            {
                Name = name.ToString(),
                TypeId = 1,
                Actions = GetAction(actorId, request)
            };

            return item;
        }

        private TodolistItem GetChangeShiftRequest(int actorId, RequestEntity request)
        {
            var details = new StringBuilder();
            if (request.ChangeShiftTypeId == (byte)ChangeShiftType.MoveOffDay)
            {
                foreach (ChangeShiftDetail changeOffDate in request.ChangeShiftDetails)
                {
                    var date = GetMessageForMoveOffDay(changeOffDate.OnDate, changeOffDate.ToDate ?? changeOffDate.OnDate);
                    details.AppendFormat("<span class='request change-shift'>{0}</span>", date);
                }

                details.Append("Move off day");
            }
            else
            {
                if (request.FromDate.Date == request.ToDate.Date)
                {
                    details.AppendFormat(
                            "<span class='request change-shift'>{0} → {1}</span> {2}",
                            request.FromShiftName,
                            request.ToShiftName,
                            request.FromDate.ToDateMonthYearFormatedDate());
                }
                else
                {
                    var date = Utilities.GetMessageDateString(request.FromDate, request.ToDate);
                    details.AppendFormat("<span class='request change-shift'>{0}</span>", date);
                }
            }

            var name = new StringBuilder();
            name.AppendFormat("<div>You have a Change Shift Request of <strong>{0} ({1})</strong> {2}</div>",
                request.FullName,
                request.Nickname,
                details);
            AppendStatus(name, request.StatusId);
            AppendRemark(name, request.Remark);

            var item = new TodolistItem
            {
                Name = name.ToString(),
                TypeId = 1,
                Actions = GetAction(actorId, request)
            };

            return item;
        }

        private TodolistItem GetCheckInOutRequest(int actorId, RequestEntity request)
        {
            var template = "<span class='request " + (request.RequestTypeId == (byte)RequestType.CheckIn ? "check-in" : "check-out") + "'>{0} {1}</span> {2}";
            var time = request.RequestedDate.Day == request.FromDate.Day ? request.RequestedDate.ToShortTime() : (request.RequestedDate.ToShortTime() + "+");

            var name = new StringBuilder();
            name.AppendFormat("<div>You have a {0} Request of <strong>{1} ({2})</strong> {3}</div>",
                request.RequestTypeId == (byte)RequestType.CheckIn ? "Check In" : "Check Out",
                request.FullName,
                request.Nickname,
                string.Format(template, time, request.FromShiftName, request.FromDate.ToDateMonthYearFormatedDate()));
            AppendStatus(name, request.StatusId);
            AppendRemark(name, request.Remark);

            var item = new TodolistItem
            {
                Name = name.ToString(),
                TypeId = 1,
                Actions = GetAction(actorId, request)
            };

            return item;
        }

        private TodolistItem GetLateInRequest(int actorId, RequestEntity request)
        {
            var details = string.Format("<span class='request late-in'>+{0}' {1}</span> {2} (Actual: {3})",
                request.InOutMinutes,
                request.FromShiftName,
                Utilities.GetMessageDateString(request.FromDate, request.ToDate),
                request.ActualInOutMinutes == 0 ? "N/A" : ("+" + request.ActualInOutMinutes + "'"));

            var name = new StringBuilder();
            name.AppendFormat("<div>You have a Late In Request of <strong>{0} ({1})</strong> {2}</div>",
                request.FullName,
                request.Nickname,
                details);
            AppendStatus(name, request.StatusId);
            AppendRemark(name, request.Remark);

            var item = new TodolistItem
            {
                Name = name.ToString(),
                TypeId = 1,
                Actions = GetAction(actorId, request)
            };

            return item;
        }

        private TodolistItem GetEarlyOutRequest(int actorId, RequestEntity request)
        {
            var details = string.Format("<span class='request early-out'>-{0}' {1}</span> {2} (Actual: {3})",
                request.InOutMinutes,
                request.FromShiftName,
                Utilities.GetMessageDateString(request.FromDate, request.ToDate),
                request.ActualInOutMinutes == 0 ? "N/A" : ("-" + request.ActualInOutMinutes + "'"));

            var name = new StringBuilder();
            name.AppendFormat("<div>You have a Early Out Request of <strong>{0} ({1})</strong> {2}</div>",
                request.FullName,
                request.Nickname,
                details);
            AppendStatus(name, request.StatusId);
            AppendRemark(name, request.Remark);

            var item = new TodolistItem
            {
                Name = name.ToString(),
                TypeId = 1,
                Actions = GetAction(actorId, request)
            };

            return item;
        }

        private TodolistItem GetKeepInViewRequest(int actorId, RequestEntity request)
        {
            var details = new StringBuilder();
            if (request.KeepInViewTypeId == (byte)KeepInViewTypes.KeepInViewByDay)
            {
                if (request.TotalDays.Equals(Constants.HalfDayOffValue))
                {
                    details.AppendFormat(
                        "<span class='request keepinview'>{0} ({1} → {2})</span> 0.5 day",
                        request.FromDate.ToDateMonthYearFormatedDate(),
                        request.FromTime.Value.ToShortTime(),
                        request.ToTime.Value.ToShortTime());
                }
                else
                {
                    details.AppendFormat("<span class='request keepinview'>{0}</span> 1 day", request.FromDate.ToDateMonthYearFormatedDate());
                }
            }
            else
            {
                var minutes = (request.ToTime.Value - request.FromTime.Value).TotalMinutes;
                var hours = minutes / Constants.TotalMinuteInHour;
                details.AppendFormat(
                    "<span class='request keepinview'>{0} ({1} → {2})</span> {3} hour(s)",
                    request.FromDate.ToDateMonthYearFormatedDate(),
                    request.FromTime.Value.ToShortTime(),
                    request.ToTime.Value.ToShortTime(),
                    hours.ToString("0.#"));
            }

            var name = new StringBuilder();
            name.AppendFormat("<div>You have a Compensation Leave Request of <strong>{0} ({1})</strong> {2}</div>",
                request.FullName,
                request.Nickname,
                details);
            AppendStatus(name, request.StatusId);
            AppendRemark(name, request.Remark);

            var item = new TodolistItem
            {
                Name = name.ToString(),
                TypeId = 1,
                Actions = GetAction(actorId, request)
            };

            return item;
        }

        private IList<TodolistAction> GetAction(int actorId, RequestEntity request)
        {
            var actions = new List<TodolistAction>();
            string url = $"{ConfigSettings.ApplicationDomain}/api/request/processRequest";
            const string method = "POST";
            if ((request.StatusId == (byte)CurrentRequestStatus.Pending && request.ApproverId == actorId)
                || request.StatusId == (byte)CurrentRequestStatus.Verified)
            {
                actions.Add(new TodolistAction
                {
                    Name = "Approve",
                    Url = url,
                    Method = method,
                    Metadata = new TodolistMetadata
                    {
                        RequestId = request.RequestId,
                        RequestTypeId = request.RequestTypeId,
                        EmpId = request.EmpId,
                        StatusId = (byte)CurrentRequestStatus.Approved,
                        Disciplines = GetDiscipline(request)
                    }
                });

                actions.Add(new TodolistAction
                {
                    Name = "Reject",
                    Url = url,
                    Method = method,
                    Metadata = new TodolistMetadata
                    {
                        RequestId = request.RequestId,
                        RequestTypeId = request.RequestTypeId,
                        EmpId = request.EmpId,
                        StatusId = (byte)CurrentRequestStatus.Rejected
                    }
                });
            }
            else
            {
                actions.Add(new TodolistAction
                {
                    Name = "Verify",
                    Url = url,
                    Method = method,
                    Metadata = new TodolistMetadata
                    {
                        RequestId = request.RequestId,
                        RequestTypeId = request.RequestTypeId,
                        EmpId = request.EmpId,
                        StatusId = (byte)CurrentRequestStatus.Verified
                    }
                });

                actions.Add(new TodolistAction
                {
                    Name = "Reject",
                    Url = url,
                    Method = method,
                    Metadata = new TodolistMetadata
                    {
                        RequestId = request.RequestId,
                        RequestTypeId = request.RequestTypeId,
                        EmpId = request.EmpId,
                        StatusId = (byte)CurrentRequestStatus.NotVerified
                    }
                });
            }

            return actions;
        }

        private IList<TodolistDiscipline> GetDiscipline(RequestEntity request)
        {
            IList<TodolistDiscipline> disciplines = new List<TodolistDiscipline>();
            switch (request.RequestTypeId)
            {
                case (byte)RequestType.Leave:
                    disciplines = GetDisciplineForLeaveRequest(request);
                    break;

                case (byte)RequestType.ChangeShift:
                    disciplines = GetDisciplineForChangeShiftRequest(request);
                    break;

                case (byte)RequestType.CheckIn:
                case (byte)RequestType.CheckOut:
                    disciplines = GetDisciplineForCheckInOutRequest(request);
                    break;

                case (byte)RequestType.LateIn:
                case (byte)RequestType.EarlyOut:
                    disciplines = GetDisciplineForLateInEarlyOutRequest(request);
                    break;

                default:
                    // nothing
                    break;
            }

            return disciplines;
        }

        private IList<TodolistDiscipline> GetDisciplineForLeaveRequest(RequestEntity request)
        {
            var disciplines = new List<TodolistDiscipline>();
            IList<Models.Request.LeaveDisciplineModel> details = requestService.GetLeaveRequestDetailToReviewDiscipline(request.RequestId);
            foreach (var item in details)
            {
                if (item.LeaveDetail.Any() && !item.IsOffDayOrHoliday)
                {
                    var isUnpaid = item.IsUnpaidLeave || item.IsUnpaidLeaveAbsent;
                    if (isUnpaid || item.IsAbsent || item.IsUnplannedLeave || item.IsSevenDaysLeave)
                    {
                        var leaveDetail = item.LeaveDetail[0];
                        disciplines.Add(new TodolistDiscipline
                        {
                            WorkDate = item.WorkDate.Date,
                            TypeId = (byte)item.DisciplineTypeId,
                            Name = item.DisciplineTypeName,
                            Details = string.Format("Leave Type: {0}", leaveDetail.LeaveTypeName)
                        });
                    }

                    if (item.IsWeekendLeave)
                    {
                        disciplines.Add(new TodolistDiscipline
                        {
                            WorkDate = item.WorkDate.Date,
                            TypeId = (byte)DisciplineType.WeekendLeave,
                            Name = EnumHelper.GetDescription(DisciplineType.WeekendLeave),
                            Details = string.Format("Leave Type: {0}", Labels.WeekendLeave)
                        });
                    }
                }
            }

            return disciplines;
        }

        private IList<TodolistDiscipline> GetDisciplineForChangeShiftRequest(RequestEntity request)
        {
            var disciplines = new List<TodolistDiscipline>();
            IList<Models.Request.ChangeShift.ChangeShiftDetail> details = requestService.GetChangeShiftDetailToReviewDiscipline(request.RequestId);
            if (details != null && details.Any())
            {
                foreach (var item in details)
                {
                    var fromShift = item.FromShiftId.Equals(-1) ? Labels.Off : item.FromShift;
                    var toShift = item.ToShiftId.Equals(-1) ? Labels.Off : item.ToShift;
                    disciplines.Add(new TodolistDiscipline
                    {
                        WorkDate = item.OnDate.Date,
                        TypeId = (byte)DisciplineType.ChangeShift,
                        Name = EnumHelper.GetDescription(DisciplineType.ChangeShift),
                        Details = string.Format("<div>- Shift: {0} → {1}</div>{2}",
                            fromShift,
                            toShift,
                            !string.IsNullOrWhiteSpace(item.Remark) ? $"<div>- Remark: {item.Remark}" : string.Empty)
                    });
                }
            }

            return disciplines;
        }

        private IList<TodolistDiscipline> GetDisciplineForCheckInOutRequest(RequestEntity request)
        {
            var disciplines = new List<TodolistDiscipline>();
            Models.Request.RequestEntity detail = requestService.GetCheckInOutDetailToReviewDiscipline(request.RequestId);
            if (detail != null)
            {
                var reason = string.Empty;
                switch (detail.ReasonId)
                {
                    case (byte)CheckInOutReason.VanLate:
                        reason = Labels.VanLate;
                        break;

                    case (byte)CheckInOutReason.BusinessTrip:
                        reason = Labels.BusinessTrip;
                        break;

                    case (byte)CheckInOutReason.Forgot:
                        reason = Labels.Forgot;
                        break;

                    case (byte)CheckInOutReason.Other:
                        reason = Labels.Other;
                        break;

                    default:
                        // nothing
                        break;
                }

                disciplines.Add(new TodolistDiscipline
                {
                    WorkDate = detail.RequestedDate.Date,
                    TypeId = (byte)DisciplineType.MissingCheckInOut,
                    Name = EnumHelper.GetDescription(DisciplineType.MissingCheckInOut),
                    Details = string.Format("<div>- Request Type: {0}</div>{1}<div>- Request Time: {2}</div><div>- Reason: {3}</div>",
                        detail.RequestType,
                        detail.FromShift != null ? $"<div>- Shift: {detail.FromShift.Name}</div>" : string.Empty,
                        detail.RequestedDate.ToShortFormatedDate(),
                        reason)
                });
            }

            return disciplines;
        }

        private IList<TodolistDiscipline> GetDisciplineForLateInEarlyOutRequest(RequestEntity request)
        {
            var disciplines = new List<TodolistDiscipline>();
            Models.Request.RequestEntity detail = requestService.GetLateInEarlyOutDetailToReviewDiscipline(request.RequestId);
            if (detail != null)
            {
                var reason = string.Empty;
                switch (detail.ReasonId)
                {
                    case (byte)LateInEarlyOutReason.ManagementAllow:
                        reason = Labels.ManagementAllow;
                        break;

                    case (byte)LateInEarlyOutReason.Other:
                        reason = Labels.Other;
                        break;

                    default:
                        // nothing
                        break;
                }

                var disciplineType = detail.RequestType.Id == (byte)RequestType.LateIn ? DisciplineType.LateIn : DisciplineType.EarlyOut;
                disciplines.Add(new TodolistDiscipline
                {
                    WorkDate = detail.FromDate.Date,
                    TypeId = (byte)disciplineType,
                    Name = EnumHelper.GetDescription(disciplineType),
                    Details = string.Format("{0}<div>- {1}: {2}'</div><div>- Reason: {3}</div>",
                        detail.FromShift != null ? $"<div>- Shift: {detail.FromShift.Name}</div>" : string.Empty,
                        detail.RequestType,
                        detail.TotalMins,
                        reason)
                });
            }

            return disciplines;
        }

        private static string GetDateTimeForLeaveRequest(RequestEntity request)
        {
            var leaveDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);

            if ((request.TotalDays - Constants.HalfDayOffValue) == 0)
            {
                leaveDate = string.Format(
                    "{0} ({1} → {2})",
                    request.FromDate.ToDateMonthYearFormatedDate(),
                    request.FromTime.Value.ToShortTime() + ((request.FromTime.Value - request.FromDate).Days > 0 ? "+" : string.Empty),
                    request.ToTime.Value.ToShortTime() + ((request.ToTime.Value - request.FromDate).Days > 0 ? "+" : string.Empty));
            }

            return leaveDate;
        }

        private static string GetLeaveDayDetail(RequestEntity request)
        {
            var result = string.Empty;
            if (request.LeaveTypeId.HasValue)
            {
                switch (request.LeaveTypeId.Value)
                {
                    case (short)LeaveTypes.GeneralLeave:
                        result = request.LeaveCountDetails;
                        break;

                    case (short)LeaveTypes.KeepInViewByHour:
                    case (short)LeaveTypes.BusinessDuringWorking:
                        var minutes = (request.ToTime.Value - request.FromTime.Value).TotalMinutes;
                        result = $"{minutes / Constants.TotalMinuteInHour:0.#} hour(s)";
                        break;

                    default:
                        result = GetTotalDay(request.TotalDays);
                        break;
                }
            }

            return result;
        }

        private static string GetTotalDay(decimal totalDays)
        {
            return totalDays != 0 ? string.Format("{0} day(s)", totalDays.ToString("0.#")) : string.Empty;
        }

        private static string GetMessageForMoveOffDay(DateTime fromDate, DateTime toDate)
        {
            if (fromDate.Year != toDate.Year)
            {
                return fromDate.ToDateMonthYearFormatedDate() + " → " + toDate.ToDateMonthYearFormatedDate();
            }

            if (fromDate.Month != toDate.Month)
            {
                return fromDate.ToString("dd") + " → " + toDate.ToDateMonthYearFormatedDate();
            }

            return fromDate.ToString("dd") + " → " + toDate.ToDateMonthYearFormatedDate();
        }

        private static void AppendStatus(StringBuilder builder, byte statusId)
        {
            builder.AppendFormat("<div><strong>Status</strong>: {0}</div>", EnumHelper.GetDescription((CurrentRequestStatus)statusId));
        }

        private static void AppendRemark(StringBuilder builder, string remark)
        {
            builder.AppendFormat("<div><strong>Remark</strong>: {0}</div>", remark);
        }
    }
}