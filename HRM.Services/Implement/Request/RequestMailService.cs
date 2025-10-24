using System;
using System.Threading.Tasks;
using HRM.Core;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Criteria.Request;
using HRM.Models;
using HRM.Models.LetsTalk;
using HRM.Models.Request;
using HRM.Models.Supervisor;
using HRM.Repositories.Interfaces;
using HRM.Resources;
using HRM.Services.Interfaces.Request;

namespace HRM.Services.Implement.Request;

public class RequestMailService(IRequestRepository requestRepository, LetstalkSettings letstalkSettings) : IRequestMailService
{
    #region constant

    private const string MailContentLeave = "{0} to {1} ({2} days)";
    private const string MailContentLeaveOneDay = "{0}";
    private const string MailContentLeaveHalfDay = "{0} (0.5 day from {1} to {2})";

    #endregion constant

    public async Task<EmailModel> GetMailToMailCcAndLetStalk(long requestId, int action, int? requestType = null)
    {
        var criteria = new GetRequestRelatedPeopleCriteria
        {
            RequestId = requestId,
            Action = action
        };
        var relatedPeople = await requestRepository.GetRequestRelatedPeople(criteria);
        var emailModel = new EmailModel
        {
            Action = action,
            RequestType = requestType,
            RequestId = requestId,
            CallbackUrl = letstalkSettings.HRMCallbackUrl,
            CompanyId = relatedPeople.EmailSetting?.CompanyId ?? 0,
        };
        GetMailAndLetstalkApprover(relatedPeople, emailModel);

        GetMailAndLetstalkVerifier(relatedPeople, emailModel);

        GetMailAndLetstalkOwnerRequest(relatedPeople, emailModel);

        if (relatedPeople.EmailSetting is { AllowSendMailGroup: true })
        {
            GetMailGroup(relatedPeople, emailModel);
        }

        emailModel.IsAllowSendRequester = relatedPeople.EmailSetting is { AllowSendUser: true };

        return emailModel;
    }

    public string GetStringDateForLeaveRequest(DateTime requestedDate, DateTime fromDate, DateTime toDate, decimal totalDays)
    {
        return totalDays switch
        {
            > 1 => string.Format(MailContentLeave, fromDate.ToDateMonthYearFormatedDate(),
                toDate.ToDateMonthYearFormatedDate(), totalDays),
            1 => string.Format(MailContentLeaveOneDay, fromDate.ToDateMonthYearFormatedDate()),
            _ => string.Format(MailContentLeaveHalfDay, requestedDate.ToDateMonthYearFormatedDate(),
                fromDate.ToClientTimeZone().ToShortTime(), toDate.ToClientTimeZone().ToShortTime())
        };
    }

    public byte GetActionProcessRequest(byte requestStatusId, RequestCommand command)
    {
        var action = command switch
        {
            RequestCommand.Accept => (byte)SettingsSendMail.Verify,
            RequestCommand.NotAccept => (byte)SettingsSendMail.NotVerify,
            RequestCommand.Approve => (byte)SettingsSendMail.Approve,
            RequestCommand.Reject => (byte)SettingsSendMail.Reject,
            RequestCommand.Process => (byte)SettingsSendMail.Process,
            RequestCommand.Void => (byte)SettingsSendMail.Void,
            RequestCommand.Reset => GetActionResetRequest(requestStatusId),
            _ => (byte)SettingsSendMail.New
        };

        return action;
    }

    public string GetCommandMessage(RequestCommand command)
    {
        var message = command switch
        {
            RequestCommand.Accept => string.Format(Messages.HasBeen, Labels.Accepted.ToLowerInvariant()),
            RequestCommand.NotAccept => string.Format(Messages.HasNotBeen, Labels.Accepted.ToLowerInvariant()),
            RequestCommand.Approve => string.Format(Messages.HasBeen, Labels.Approved.ToLowerInvariant()),
            RequestCommand.Reject => string.Format(Messages.HasBeen, Labels.Rejected.ToLowerInvariant()),
            RequestCommand.Process => string.Format(Messages.HasBeen, Labels.Processed.ToLowerInvariant()),
            RequestCommand.Void => string.Format(Messages.HasBeen, Labels.Void.ToLowerInvariant()),
            RequestCommand.Reset => string.Format(Messages.HasBeen, Labels.Reseted.ToLowerInvariant()),
            _ => string.Format(Messages.HasBeen, Labels.Void.ToLowerInvariant())
        };

        return message;
    }

    private static byte GetActionResetRequest(byte statusId)
    {
        var action = statusId switch
        {
            (byte)CurrentRequestStatus.Verified or (byte)CurrentRequestStatus.NotVerified => (byte)SettingsSendMail.Reset1,
            (byte)CurrentRequestStatus.Approved or (byte)CurrentRequestStatus.Rejected => (byte)SettingsSendMail.Reset2,
            _ => (byte)SettingsSendMail.Reset3
        };

        return action;
    }

    private static void GetMailAndLetstalkApprover(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
    {
        var approver = relatedPeople.Approver;
        if (approver == null || relatedPeople.EmailSetting == null || !relatedPeople.EmailSetting.AllowSendApprover)
        {
            return;
        }
        emailModel.SendTo = approver.Email;
        emailModel.SendToNickName = approver.NickName;

        if (!string.IsNullOrWhiteSpace(approver.LetsTalk)
        && approver.EmpId != relatedPeople.Requester?.EmpId)
        {
            emailModel.Approver = GenerateLetstalkActor(emailModel, approver, true);
        }
    }

    private static void GetMailAndLetstalkVerifier(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
    {
        var isValidVerifier = relatedPeople.Verifier != null;
        var isValidApprover = relatedPeople.Approver != null;

        if (relatedPeople.EmailSetting is not { AllowSendVerifier: true } || !isValidVerifier)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(emailModel.SendTo))
        {
            emailModel.SendTo = relatedPeople.Verifier.Email;
            emailModel.SendToNickName = relatedPeople.Verifier.NickName;
        }
        else
        {
            var isValidEmail = isValidApprover
                   && !string.IsNullOrWhiteSpace(relatedPeople.Verifier.Email)
                   && !relatedPeople.Verifier.Email.Equals(relatedPeople.Approver.Email, StringComparison.OrdinalIgnoreCase);
            if (isValidEmail)
            {
                emailModel.SendTo += ";" + relatedPeople.Verifier.Email;
            }
        }

        if (IsValidLetstalkVerifier(relatedPeople))
        {
            emailModel.Verifier = GenerateLetstalkActor(emailModel, relatedPeople.Verifier, false);
        }
    }

    private static bool IsValidLetstalkVerifier(RequestRelatedPeopleModel relatedPeople)
        => !string.IsNullOrWhiteSpace(relatedPeople.Verifier.LetsTalk)
           && relatedPeople.Verifier.EmpId != relatedPeople.Approver?.EmpId
           && relatedPeople.Verifier.EmpId != relatedPeople.Requester?.EmpId;

    private static void GetMailAndLetstalkOwnerRequest(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
    {
        emailModel.RequesterCode = relatedPeople.Requester?.EmpCode;
        emailModel.PICDisplayName = relatedPeople.Requester?.PICDisplayName;
        if (relatedPeople.EmailSetting is { AllowSendUser: true } && relatedPeople.Requester != null)
        {
            if (string.IsNullOrWhiteSpace(emailModel.SendTo))
            {
                emailModel.SendTo = relatedPeople.Requester.Email;
                emailModel.SendToNickName = relatedPeople.Requester.NickName;
            }
            else
            {
                emailModel.SendCC = AddMailToList(emailModel.SendCC, relatedPeople.Requester.Email);
            }
        }

        if (relatedPeople.Requester != null && !string.IsNullOrWhiteSpace(relatedPeople.Requester.LetsTalk))
        {
            emailModel.Owner = new LetsTalkActor
            {
                LetsTalkId = relatedPeople.Requester.LetsTalk
            };
        }
    }

    private static string AddMailToList(string mailList, string mail)
    {
        if (string.IsNullOrWhiteSpace(mail))
        {
            return mailList;
        }

        if (string.IsNullOrWhiteSpace(mailList))
        {
            return mail;
        }

        return mailList + ";" + mail;
    }

    private static void GetMailGroup(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
    {
        if (relatedPeople.Requester != null)
        {
            if (string.IsNullOrWhiteSpace(emailModel.SendTo))
            {
                emailModel.SendTo = relatedPeople.Requester.MailGroup;
                emailModel.SendToNickName = Labels.All;
            }
            else if (string.IsNullOrWhiteSpace(emailModel.SendCC))
            {
                emailModel.SendCC = relatedPeople.Requester.MailGroup;
            }
            else
            {
                emailModel.SendCC += ";" + relatedPeople.Requester.MailGroup;
            }
        }
    }

    private static LetsTalkActor GenerateLetstalkActor(EmailModel emailModel, SupervisorNameModel supervisor, bool isApprover)
    {
        var allowReplyMarkup = emailModel.Action is (byte)SettingsSendMail.New or (byte)SettingsSendMail.Edit;
        if (!allowReplyMarkup)
        {
            return new LetsTalkActor
            {
                LetsTalkId = supervisor.LetsTalk
            };
        }

        var roleId = isApprover ? "3" : "2";
        var textAccept = isApprover ? "Approve" : "Verify";
        var command = isApprover ? RequestCommand.Approve : RequestCommand.Accept;

        var param = BuildRequestParameters(emailModel, supervisor, roleId);
        return new LetsTalkActor
        {
            LetsTalkId = supervisor.LetsTalk,
            ReplyMarkup = new ReplyMarkup
            {
                Actions =
                {
                    new ReplyAction { ActionType = ReplyActionType.PostBack, Label = textAccept, Data = $"{emailModel.CallbackUrl}api/request/process?command={command}{param}" },
                    new ReplyAction { ActionType = ReplyActionType.PostBack, Label = "Reject", Data = $"{emailModel.CallbackUrl}api/request/process?command={RequestCommand.Reject}{param}" },
                    new ReplyAction { ActionType = ReplyActionType.Uri, Label = "View detail", Uri = $"{emailModel.CallbackUrl}request/member" }
                }
            }
        };
    }

    private static string BuildRequestParameters(EmailModel emailModel, SupervisorNameModel supervisor, string roleId)
    {
        return $"&requestId={emailModel.RequestId}" +
               $"&requestTypeId={emailModel.RequestType}" +
               $"&roleId={roleId}" +
               $"&empId={supervisor.EmpId}" +
               $"&nickName={supervisor.NickName}" +
               $"&fullName={supervisor.FullName}" +
               $"&companyId={emailModel.CompanyId}";
    }
}