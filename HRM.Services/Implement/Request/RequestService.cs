namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Core.XMLExtensions;
    using HRM.Criteria.Request;
    using HRM.Criteria.Request.ChangeShift;
    using HRM.Criteria.Request.CheckInOut;
    using HRM.Criteria.Request.LateInEarlyOut;
    using HRM.Criteria.Request.Leave;
    using HRM.Models.Request;
    using HRM.Models.Request.ChangeShift;
    using HRM.Repositories;
    using HRM.Resources;

    public class RequestService : IRequestService
    {
        private readonly IRequestRepository requestRepository;
        private readonly IBaseService baseService;
        private readonly ICheckInOutRequestService checkInOutRequestService;
        private readonly ILateInEarlyOutRequestService lateInEarlyOutRequestService;
        private readonly IChangeShiftRequestService changeShiftRequestService;
        private readonly ILeaveRequestService leaveRequestService;
        private readonly IKeepInViewRequestService keepInViewRequestService;

        public RequestService(
            IRequestRepository requestRepository,
            IBaseService baseService,
            ICheckInOutRequestService checkInOutRequestService,
            ILateInEarlyOutRequestService lateInEarlyOutRequestService,
            IChangeShiftRequestService changeShiftRequestService,
            ILeaveRequestService leaveRequestService,
            IKeepInViewRequestService keepInViewRequestService)
        {
            this.requestRepository = requestRepository;
            this.baseService = baseService;
            this.checkInOutRequestService = checkInOutRequestService;
            this.lateInEarlyOutRequestService = lateInEarlyOutRequestService;
            this.changeShiftRequestService = changeShiftRequestService;
            this.leaveRequestService = leaveRequestService;
            this.keepInViewRequestService = keepInViewRequestService;
        }

        public static string ToRequestMessage(RequestEntity request, bool isNoFormat = false)
        {
            string message;
            switch (request.RequestType.Id)
            {
                case (byte)RequestType.Leave:
                    message = GetMessageForLeave(request, isNoFormat);
                    break;

                case (byte)RequestType.ChangeShift:
                    message = GetMessageForChangeShift(request, isNoFormat);
                    break;

                case (byte)RequestType.CheckIn:
                case (byte)RequestType.CheckOut:
                    message = GetMessageForCheckInOut(request, isNoFormat);
                    break;

                case (byte)RequestType.LateIn:
                    message = GetMessageForLateIn(request, isNoFormat);
                    break;

                case (byte)RequestType.EarlyOut:
                    message = GetMessageForEarlyOut(request, isNoFormat);
                    break;

                case (byte)RequestType.KeepInView:
                    message = GetMessageForKeepInView(request, isNoFormat);
                    break;

                default:
                    message = string.Empty;
                    break;
            }

            return message;
        }

        public static string GetReasonRequest(short requestTypeId, byte reasonId)
        {
            var reason = string.Empty;
            if (requestTypeId == (byte)RequestType.CheckIn || requestTypeId == (byte)RequestType.CheckOut)
            {
                switch (reasonId)
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
            }
            else
            {
                switch (reasonId)
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
            }

            return reason;
        }

        public IList<RequestEntity> GetRequests(GetRequestCriteria criteria)
        {
            IList<RequestEntity> entityList = null;
            if (criteria.IsValid())
            {
                entityList = requestRepository.GetRequests(criteria);
            }

            return entityList;
        }

        public VerifierAndApproverForRequest GetVerifierAndApprover(int employeeId, RequestType requestType)
        {
            var criteria = new GetVerifierAndApproverCriteria
            {
                EmpId = employeeId,
                RequestType = (byte)requestType
            };
            VerifierAndApproverForRequest supervisor = requestRepository.GetVerifierAndApprover(criteria);
            return supervisor;
        }

        public RequestEntity GetCheckInOutDetailToReviewDiscipline(long requestId)
        {
            var criteria = new GetCheckInOutDetailToReviewDisciplineCriteria
            {
                RequestId = requestId
            };
            return requestRepository.GetRequestToIgnoreById(criteria);
        }

        public RequestEntity GetLateInEarlyOutDetailToReviewDiscipline(long requestId)
        {
            var criteria = new GetLateInEarlyOutDetailToReviewDisciplineCriteria
            {
                RequestId = requestId
            };
            return requestRepository.GetRequestToIgnoreById(criteria);
        }

        public IList<ChangeShiftDetail> GetChangeShiftDetailToReviewDiscipline(long requestId)
        {
            var criteria = new GetChangeShiftDetailToReviewDisciplineCriteria
            {
                RequestId = requestId
            };
            return baseService.GetData<ChangeShiftDetail>(criteria).ToList();
        }

        public IList<LeaveDisciplineModel> GetLeaveRequestDetailToReviewDiscipline(long requestId)
        {
            var criteria = new GetLeaveRequestDetailToReviewDisciplineCriteria
            {
                RequestId = requestId
            };
            return requestRepository.GetLeaveRequestToReviewDiscipline(criteria);
        }

        public CommandResult ApproveRequest(int actorId, ProcessRequestParam param)
        {
            var criteria = new ApproveRequestCriteria
            {
                RequestId = param.RequestId,
                RoleId = param.RoleId,
                Comment = param.Comment,
                ActorId = actorId,
                DisciplineXml = param.Disciplines.Any() ? param.Disciplines.ToXML().ToString() : string.Empty,
                ViewType = param.ViewType
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult ApproveRequests(ApproveRequestsParam param)
        {
            var criteria = new ApproveMultipleRequestsCriteria
            {
                RoleId = param.RoleId,
                Comment = param.Comment,
                ActorId = param.ActorId,
                IsIgnoredViolate = param.IsIgnoredViolate,
                IsConfirmed = param.IsConfirmed,
                Requests = param.Requests.Any() ? param.Requests.ToXML().ToString() : string.Empty
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult RejectRequest(int actorId, ProcessRequestParam param)
        {
            var criteria = new RejectRequestCriteria
            {
                RequestId = param.RequestId,
                RequestTypeId = param.RequestTypeId,
                RoleId = param.RoleId,
                Comment = param.Comment,
                ActorId = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult ResetRequest(int actorId, ProcessRequestParam param)
        {
            var criteria = new ResetRequestCriteria
            {
                RequestId = param.RequestId,
                RequestTypeId = param.RequestTypeId,
                RoleId = param.RoleId,
                ActorId = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult VoidRequest(int actorId, ProcessRequestParam param)
        {
            var criteria = new VoidRequestCriteria
            {
                RequestId = param.RequestId,
                RequestTypeId = param.RequestTypeId,
                Comment = param.Comment,
                ActorId = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public void SendMailProcessRequest(ProcessRequestParam param, EmployeeSession actor, RequestCommand command)
        {
            switch (param.RequestTypeId)
            {
                case RequestType.CheckIn:
                case RequestType.CheckOut:
                    checkInOutRequestService.SendMailProcessRequest(param.RequestId, actor, command);
                    break;

                case RequestType.LateIn:
                case RequestType.EarlyOut:
                    lateInEarlyOutRequestService.SendMailProcessRequest(param.RequestId, actor, command);
                    break;

                case RequestType.ChangeShift:
                    changeShiftRequestService.SendMailProcessRequest(param.RequestId, actor, command);
                    break;

                case RequestType.Leave:
                    leaveRequestService.SendMailProcessRequest(param.RequestId, actor, command);
                    break;

                default:
                    keepInViewRequestService.SendMailProcessRequest(param.RequestId, actor, command);
                    break;
            }
        }

        public void SendMailProcessRequests(ApproveRequestsParam param, EmployeeSession actor, RequestCommand command)
        {
            foreach (var request in param.Requests)
            {
                switch (request.RequestTypeID)
                {
                    case (int)RequestType.CheckIn:
                    case (int)RequestType.CheckOut:
                        checkInOutRequestService.SendMailProcessRequest(request.RequestId, actor, command);
                        break;

                    case (int)RequestType.LateIn:
                    case (int)RequestType.EarlyOut:
                        lateInEarlyOutRequestService.SendMailProcessRequest(request.RequestId, actor, command);
                        break;

                    case (int)RequestType.ChangeShift:
                        changeShiftRequestService.SendMailProcessRequest(request.RequestId, actor, command);
                        break;

                    case (int)RequestType.Leave:
                        leaveRequestService.SendMailProcessRequest(request.RequestId, actor, command);
                        break;

                    default:
                        keepInViewRequestService.SendMailProcessRequest(request.RequestId, actor, command);
                        break;
                }
            }
        }

        private static string GetMessageForLeave(RequestEntity request, bool isNoFormat)
        {
            var msgTemplate = isNoFormat ? RequestFormatConstants.MsgLeaveWithNoFormat : RequestFormatConstants.MsgLeave;
            var leaveDateTime = GetDateTimeForLeaveRequest(request);
            var leaveDayDetail = GetLeaveDayDetail(request);
            var messageDetail = string.Format(msgTemplate, leaveDateTime, leaveDayDetail);

            return messageDetail;
        }

        private static string GetDateTimeForLeaveRequest(RequestEntity request)
        {
            var leaveDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);

            if ((request.TotalDays - Constants.HalfDayOffValue) == 0)
            {
                leaveDate = string.Format(
                    RequestFormatConstants.MsgLeaveHalfDayWithNoFormat,
                    request.FromDate.ToDateMonthYearFormatedDate(),
                    request.FromTime.ToShortTime() + ((request.FromTime - request.FromDate).Days > 0 ? "+" : string.Empty),
                    request.ToTime.ToShortTime() + ((request.ToTime - request.FromDate).Days > 0 ? "+" : string.Empty));
            }

            return leaveDate;
        }

        private static string GetMessageForChangeShift(RequestEntity request, bool isNoFormat)
        {
            string msgTemplate;
            string messageDetail;

            if (request.ChangeShiftType == (byte)ChangeShiftType.MoveOffDay)
            {
                var moveOffDayBuilder = new StringBuilder();
                foreach (var changeOffDate in request.ChangeShiftDetails)
                {
                    msgTemplate = RequestFormatConstants.MsgChangeShift;

                    var dateFomarted = GetMessageForMoveOffDay(changeOffDate.OnDate, changeOffDate.ToDate ?? changeOffDate.OnDate);
                    moveOffDayBuilder.Append(string.Format(msgTemplate, dateFomarted));
                }

                moveOffDayBuilder.Append("Move off day");
                messageDetail = moveOffDayBuilder.ToString();
            }
            else
            {
                if (request.FromDate.Date == request.ToDate.Date)
                {
                    msgTemplate = isNoFormat ? RequestFormatConstants.MsgChangeShiftOneDayWithNoFormat : RequestFormatConstants.MsgChangeShiftOneDay;
                    messageDetail = string.Format(
                            msgTemplate,
                            request.FromShift.Name,
                            request.ToShift.Name,
                            request.FromDate.ToDateMonthYearFormatedDate());
                }
                else
                {
                    msgTemplate = isNoFormat ? RequestFormatConstants.MsgChangeShiftWithNoFormat : RequestFormatConstants.MsgChangeShift;
                    var date = Utilities.GetMessageDateString(request.FromDate, request.ToDate);
                    messageDetail = string.Format(msgTemplate, date);
                }
            }

            return messageDetail;
        }

        private static string GetMessageForMoveOffDay(DateTime fromDate, DateTime toDate)
        {
            const string arrowSymbol = " <span class='arr'>&rarr;</span> ";
            if (fromDate.Year != toDate.Year)
            {
                return fromDate.ToDateMonthYearFormatedDate() + arrowSymbol + toDate.ToDateMonthYearFormatedDate();
            }

            if (fromDate.Month != toDate.Month)
            {
                return fromDate.ToString("dd") + arrowSymbol + toDate.ToDateMonthYearFormatedDate();
            }

            return fromDate.ToString("dd") + arrowSymbol + toDate.ToDateMonthYearFormatedDate();
        }

        private static string GetMessageForCheckInOut(RequestEntity request, bool isNoFormat)
        {
            var msgTemplate = isNoFormat ? RequestFormatConstants.MsgCheckInOutWithNoFormat : RequestFormatConstants.MsgCheckInOut;
            var time = request.RequestedDate.Day == request.FromDate.Day ? request.RequestedDate.ToShortTime() : (request.RequestedDate.ToShortTime() + "+");
            return string.Format(msgTemplate, time, request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
        }

        private static string GetMessageForLateIn(RequestEntity request, bool isNoFormat)
        {
            var msgTemplate = isNoFormat ? RequestFormatConstants.MsgLateInWithNoFormat : RequestFormatConstants.MsgLateIn;
            var actualLateIn = request.ActualLateInEarlyOut == 0 ? Labels.NA : ("+" + request.ActualLateInEarlyOut + "'");

            return string.Format(msgTemplate, request.TotalMins, request.FromShift.Name, Utilities.GetMessageDateString(request.FromDate, request.ToDate), actualLateIn);
        }

        private static string GetMessageForEarlyOut(RequestEntity request, bool isNoFormat)
        {
            var msgTemplate = isNoFormat ? RequestFormatConstants.MsgEarlyOutWithNoFormat : RequestFormatConstants.MsgEarlyOut;
            var actualEarlyOut = request.ActualLateInEarlyOut == 0 ? Labels.NA : ("-" + request.ActualLateInEarlyOut + "'");

            return string.Format(msgTemplate, request.TotalMins, request.FromShift.Name, Utilities.GetMessageDateString(request.FromDate, request.ToDate), actualEarlyOut);
        }

        private static string GetMessageForKeepInView(RequestEntity request, bool isNoFormat)
        {
            string messageDetail = request.KeepInViewType == (int)KeepInViewTypes.KeepInViewByDay
                ? GetMessageForKeepInViewByDay(request, isNoFormat)
                : GetMessageForKeepInViewByHour(request, isNoFormat);

            return messageDetail;
        }

        private static string GetMessageForKeepInViewByDay(RequestEntity request, bool isNoFormat)
        {
            string msgTemplate;
            string messageDetail;

            if (request.TotalDays.Equals(Constants.HalfDayOffValue))
            {
                msgTemplate = isNoFormat ? RequestFormatConstants.MsgKeepInViewHalfdayWithNoFormat : RequestFormatConstants.MsgKeepInViewHalfday;
                messageDetail = string.Format(
                    msgTemplate,
                    request.FromDate.ToDateMonthYearFormatedDate(),
                    request.FromTime.ToShortTime(),
                    request.ToTime.ToShortTime());
            }
            else
            {
                msgTemplate = isNoFormat ? RequestFormatConstants.MsgKeepInViewOneDayWithNoFormat : RequestFormatConstants.MsgKeepInViewOneDay;
                messageDetail = string.Format(msgTemplate, request.FromDate.ToDateMonthYearFormatedDate());
            }

            return messageDetail;
        }

        private static string GetMessageForKeepInViewByHour(RequestEntity request, bool isNoFormat)
        {
            var msgTemplate = isNoFormat ? RequestFormatConstants.MsgKeepInViewByHoursWithNoFormat : RequestFormatConstants.MsgKeepInViewByHours;
            var minutes = (request.ToTime - request.FromTime).TotalMinutes;
            var hours = minutes / Constants.TotalMinuteInHour;
            return string.Format(
                    msgTemplate,
                    request.FromDate.ToDateMonthYearFormatedDate(),
                    request.FromTime.ToShortTime(),
                    request.ToTime.ToShortTime(),
                    hours.ToString("0.#"));
        }

        private static string GetLeaveDayDetail(RequestEntity request)
        {
            var result = string.Empty;
            if (request.LeaveType != null)
            {
                switch (request.LeaveType.Id)
                {
                    case (ushort)LeaveTypes.GeneralLeave:
                        result = request.LeaveCountDetails;
                        break;

                    case (ushort)LeaveTypes.KeepInViewByHour:
                    case (ushort)LeaveTypes.BusinessDuringWorking:
                        var minutes = (request.ToTime - request.FromTime).TotalMinutes;
                        result = $"{(minutes / Constants.TotalMinuteInHour).ToString("0.#")} hour(s)";
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
            return totalDays != 0 ? string.Format(RequestFormatConstants.MsgTotalLeaveDays, totalDays.ToString("0.#")) : string.Empty;
        }
    }
}