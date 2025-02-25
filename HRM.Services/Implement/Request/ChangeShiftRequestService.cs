namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Core.XMLExtensions;
    using HRM.Criteria.Request.ChangeShift;
    using HRM.Models.Request.ChangeShift;
    using HRM.Models.ScheduleArrange;
    using HRM.Repositories;
    using HRM.Resources;
    using HRM.Services.Interfaces;

    public class ChangeShiftRequestService(
        AppSettings appSettings,
        IChangeShiftRequestRepository changeShiftRepository,
        IBaseService baseService,
        IEmailService emailService,
        IRequestMailService requestMailService,
        ILetstalkService letstalkService) : IChangeShiftRequestService
    {
        private const string EmailTemplatePathForRequest = "~/app_data/emails/ChangeShift.Request.txt";
        private const string EmailTemplatePathForCancel = "~/app_data/emails/ChangeShift.Cancel.txt";
        private const string EmailTemplatePathForApproval = "~/app_data/emails/ChangeShift.Approval.txt";

        public int GetNumberOfRequest(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetNumberOfRequestCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };
            return baseService.GetDataEntity<int>(criteria);
        }

        public IEnumerable<DailySchedule> GetWorkingCalendarForRequest(GetWorkingCalendarForRequestParam param)
        {
            var criteria = new GetWorkingCalendarForRequestCriteria
            {
                EmployeeId = param.EmployeeId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                TypeId = param.TypeId,
                RequestId = param.RequestId
            };
            return changeShiftRepository.GetWorkingCalendarForRequest(criteria);
        }

        public IEnumerable<DateTime> GetDisableDateForMoveOffDayRequest(int employeeId, DateTime requestDate, long? requestId = null)
        {
            var fromDate = new DateTimeOffset(requestDate.Year, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
            var criteria = new GetDisableDateForMoveOffDayRequestCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = fromDate.AddYears(1).AddDays(-1),
                RequestId = requestId
            };
            var dates = baseService.GetData<DateTime>(criteria).ToList();
            for (var day = requestDate.FirstDayOfWeek().Date; day.Date <= requestDate.LastDayOfWeek().Date; day = day.AddDays(1))
            {
                if (!dates.Any(x => x.Date == day.Date))
                {
                    dates.Add(day);
                }
            }

            return dates;
        }

        public IEnumerable<ChangeShiftRequest> GetByEmployee(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetChangeShiftRequestByEmployeeCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };
            return changeShiftRepository.GetByEmployee(criteria);
        }

        public IEnumerable<ChangeShiftRequest> GetHistoryById(long requestId)
        {
            var criteria = new GetChangeShiftRequestHistoryByIdCriteria
            {
                RequestId = requestId
            };
            return changeShiftRepository.GetHistoryById(criteria);
        }

        public ChangeShiftRequest GetById(long requestId)
        {
            var criteria = new GetChangeShiftRequestByIdCriteria
            {
                RequestId = requestId
            };
            return changeShiftRepository.GetById(criteria);
        }

        public IEnumerable<ChangeShiftDetail> GetDetailById(long requestId)
        {
            var criteria = new GetChangeShiftRequestDetailByIdCriteria
            {
                RequestId = requestId
            };
            var details = baseService.GetData<ChangeShiftDetail>(criteria);
            foreach (var detail in details)
            {
                if (detail.FromShiftId == Constants.OffDayShiftId)
                {
                    detail.FromShift = Labels.Off;
                }

                if (detail.ToShiftId == Constants.OffDayShiftId)
                {
                    detail.ToShift = Labels.Off;
                }
            }

            return details;
        }

        public CommandResult Create(int actorId, CreateChangeShift model)
        {
            var criteria = new CreateChangeShiftRequestCriteria
            {
                EmployeeId = model.EmployeeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TypeId = model.TypeId,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                Remark = model.Remark,
                DetailXml = model.Details.ToXML().ToString(),
                ActorId = actorId
            };
            var commandResult = baseService.ExecuteNonQuery(criteria);
            commandResult.OutputId = criteria.RequestId;

            return commandResult;
        }

        public CommandResult Edit(int actorId, EditChangeShift model)
        {
            var criteria = new EditChangeShiftRequestCriteria
            {
                RequestId = model.RequestId,
                EmployeeId = model.EmployeeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TypeId = model.TypeId,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                Remark = model.Remark,
                DetailXml = model.Details.ToXML().ToString(),
                ActorId = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int actorId, long requestId)
        {
            var criteria = new DeleteChangeShiftRequestCriteria
            {
                RequestId = requestId,
                ActorId = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public void SendMailAndSendLetstalkCreateRequest(long requestId)
        {
            var request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.New);
                var forDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);
                email.Subject = string.Format(
                    "{0} {1} {2}",
                    Messages.ChangeShiftEmailSubject,
                    request.Requester.DisplayName,
                    forDate);

                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.Nickname,
                    forDate,
                    request.Remark,
                    appSettings.Domain);

                Task.Run(() => emailService.SendEmail(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                Task.Run(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
            }
        }

        public void SendMailEditRequest(long requestId)
        {
            var request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForRequest);
                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.Edit);
                var forDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);
                email.Subject = string.Format(
                    "{0} {1} {2}",
                    Messages.UpdatedChangeShiftEmailSubject,
                    request.Requester.DisplayName,
                    forDate);

                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.Nickname,
                    forDate,
                    request.Remark,
                    appSettings.Domain);

                Task.Run(() => emailService.SendEmail(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                Task.Run(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
            }
        }

        public void SendMailDeleteRequest(long requestId)
        {
            var request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForCancel);
                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, (byte)SettingsSendMail.Cancel);
                var forDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);

                email.Subject = string.Format(Messages.MessageChangeShiftRequest, request.Requester.Nickname);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.DisplayName,
                    forDate,
                    request.Remark);

                Task.Run(() => emailService.SendEmail(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                Task.Run(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
            }
        }

        public void SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command)
        {
            var request = GetById(requestId);
            if (request != null)
            {
                var emailTemplatePath = HttpContext.Current.Server.MapPath(EmailTemplatePathForApproval);
                var action = requestMailService.GetActionProcessRequest(request.Status.Id, command);
                var commandMessage = requestMailService.GetCommandMessage(command);
                var email = requestMailService.GetMailToMailCCAndLetstalk(requestId, action);
                var forDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);

                email.Subject = string.Format(Messages.ApprovalNotificationChangeShiftSubject, request.Requester.Nickname, commandMessage, actor.Nickname);
                email.Body = Utilities.ReadTextFile(emailTemplatePath);
                email.Body = string.Format(
                    email.Body,
                    email.SendToNickName,
                    request.Requester.DisplayName,
                    commandMessage,
                    actor.DisplayName,
                    Labels.ChangeShift,
                    forDate,
                    request.Remark);

                Task.Run(() => emailService.SendEmail(
                    email.SendTo,
                    email.SendCC,
                    email.Subject,
                    email.Body));

                Task.Run(() => letstalkService.SendNotification(email.LetsTalk, email.Subject, email.Body));
            }
        }
    }
}