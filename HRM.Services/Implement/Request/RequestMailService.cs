using System;
using System.Threading.Tasks;
using HRM.Core;
using HRM.Core.Enums;
using HRM.Core.Services;
using HRM.Criteria.Request;
using HRM.Criteria.SystemSetting;
using HRM.Models;
using HRM.Models.Request;
using HRM.Repositories.Interfaces;
using HRM.Resources;
using HRM.Services.Interfaces.Request;

namespace HRM.Services.Implement.Request;

public class RequestMailService(IRequestRepository requestRepository,
    IBaseService baseService) : IRequestMailService
{
    #region constant

    private const string MailContentLeave = "{0} to {1} ({2} days)";
    private const string MailContentLeaveOneDay = "{0}";
    private const string MailContentLeaveHalfDay = "{0} (0.5 day from {1} to {2})";

    #endregion constant

    public async Task<EmailModel> GetMailToMailCcAndLetStalk(long requestId, int action)
    {
        var criteria = new GetRequestRelatedPeopleCriteria
        {
            RequestId = requestId,
            Action = action
        };
        var relatedPeople = await requestRepository.GetRequestRelatedPeople(criteria);

        var emailModel = GetMailAndLetStalkApprover(relatedPeople);

        GetMailAndLetStalkVerifier(relatedPeople, emailModel);

        GetMailAndLetStalkOwnerRequest(relatedPeople, emailModel);

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
                fromDate.ToShortTime(), toDate.ToShortTime())
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

    public async Task<MailConfigModel> GetEmployeeMailConfig(int empId)
    {
        GetMailConfigByEmpIdCriteria mailConfigCriteria = new()
        {
            EmpId = empId,
        };
        var mailConfig = await baseService.GetDataEntityAsync<MailConfigModel>(mailConfigCriteria);

        return mailConfig;
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

    private static EmailModel GetMailAndLetStalkApprover(RequestRelatedPeopleModel relatedPeople)
    {
        var emailModel = new EmailModel();
        var approver = relatedPeople.Approver;
        if (approver == null || relatedPeople.EmailSetting == null || !relatedPeople.EmailSetting.AllowSendApprover)
        {
            return emailModel;
        }
        emailModel.SendTo = approver.Email;
        emailModel.SendToNickName = approver.NickName;
        emailModel.LetsTalk = !string.IsNullOrWhiteSpace(approver.LetsTalk) ? approver.LetsTalk : string.Empty;

        return emailModel;
    }

    private static void GetMailAndLetStalkVerifier(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
    {
        var isValidVerifier = relatedPeople.Verifier != null;
        var isValidApprover = relatedPeople.Approver != null;

        if (relatedPeople.EmailSetting == null || !relatedPeople.EmailSetting.AllowSendVerifier || !isValidVerifier)
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

        GetLetStalkVerifier(relatedPeople, emailModel);
    }

    private static void GetLetStalkVerifier(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
    {
        if (string.IsNullOrWhiteSpace(emailModel.LetsTalk))
        {
            emailModel.LetsTalk = relatedPeople.Verifier.LetsTalk;
        }
        else
        {
            var isValidLetStalk = relatedPeople.Approver != null
                                && !string.IsNullOrWhiteSpace(relatedPeople.Verifier.LetsTalk)
                                && !relatedPeople.Verifier.LetsTalk.Equals(relatedPeople.Approver.LetsTalk, StringComparison.OrdinalIgnoreCase);
            if (isValidLetStalk)
            {
                emailModel.LetsTalk += ";" + relatedPeople.Verifier.LetsTalk;
            }
        }
    }

    private static void GetMailAndLetStalkOwnerRequest(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
    {
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

            if (string.IsNullOrWhiteSpace(emailModel.LetsTalk))
            {
                emailModel.LetsTalk = relatedPeople.Requester.LetsTalk;
            }
            else
            {
                emailModel.LetsTalk += ";" + relatedPeople.Requester.LetsTalk;
            }
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
}