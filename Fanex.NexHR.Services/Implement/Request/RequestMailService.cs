namespace Fanex.NexHR.Services
{
    using System;
    using System.Linq;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Resources;

    public class RequestMailService : IRequestMailService
    {
        #region constant

        private const string _MailContentLeave = "{0} to {1} ({2} days)";
        private const string _MailContentLeaveOneDay = "{0}";
        private const string _MailContentLeaveHalfDay = "{0} (0.5 day from {1} to {2})";

        #endregion constant

        private readonly IRequestRepository requestRepository;

        public RequestMailService(IRequestRepository requestRepository)
        {
            this.requestRepository = requestRepository;
        }

        public EmailModel GetMailToMailCCAndLetstalk(long requestId, int action)
        {
            var criteria = new GetRequestRelatedPeopleCriteria
            {
                RequestId = requestId,
                Action = action
            };
            RequestRelatedPeopleModel relatedPeople = requestRepository.GetRequestRelatedPeople(criteria);

            EmailModel emailModel = GetMailAndLetstalkApprover(relatedPeople);

            GetMailAndLetstalkVerifier(relatedPeople, emailModel);

            RelatedPeopleAllowSendHR(relatedPeople);
            GetMailAndLetstalkHR(relatedPeople, emailModel);

            GetMailAndLetstalkOwnerRequest(relatedPeople, emailModel);

            if (relatedPeople.EmailSetting.AllowSendMailGroup)
            {
                GetMailGroup(relatedPeople, emailModel);
            }

            emailModel.IsAllowSendRequester = relatedPeople.EmailSetting.AllowSendUser;
            emailModel.Sender = ConfigSettings.EmailSetting;
            return emailModel;
        }

        public string GetStringDateForLeaveRequest(DateTime requestedDate, DateTime fromDate, DateTime toDate, decimal totalDays)
        {
            if (totalDays > 1)
            {
                return string.Format(_MailContentLeave, fromDate.ToDateMonthYearFormatedDate(), toDate.ToDateMonthYearFormatedDate(), totalDays);
            }

            if (totalDays == 1)
            {
                return string.Format(_MailContentLeaveOneDay, fromDate.ToDateMonthYearFormatedDate());
            }

            return string.Format(_MailContentLeaveHalfDay, requestedDate.ToDateMonthYearFormatedDate(), fromDate.ToShortTime(), toDate.ToShortTime());
        }

        private static void RelatedPeopleAllowSendHR(RequestRelatedPeopleModel relatedPeople)
        {
            if (relatedPeople.Verifier != null)
            {
                relatedPeople.ListHR = relatedPeople.ListHR.Where(t => t.Email != relatedPeople.Verifier.Email).ToList();
            }

            if (relatedPeople.Approver != null)
            {
                relatedPeople.ListHR = relatedPeople.ListHR.Where(t => t.Email != relatedPeople.Approver.Email).ToList();
            }
        }

        public byte GetActionProcessRequest(byte requestStatusId, RequestCommand command)
        {
            byte action;
            switch (command)
            {
                case RequestCommand.Accept:
                    action = (byte)SettingsSendMail.Verify;
                    break;

                case RequestCommand.NotAccept:
                    action = (byte)SettingsSendMail.NotVerify;
                    break;

                case RequestCommand.Approve:
                    action = (byte)SettingsSendMail.Approve;
                    break;

                case RequestCommand.Reject:
                    action = (byte)SettingsSendMail.Reject;
                    break;

                case RequestCommand.Process:
                    action = (byte)SettingsSendMail.Process;
                    break;

                case RequestCommand.Void:
                    action = (byte)SettingsSendMail.Void;
                    break;

                case RequestCommand.Reset:
                    action = GetActionResetRequest(requestStatusId);
                    break;

                default:
                    action = (byte)SettingsSendMail.New;
                    break;
            }

            return action;
        }

        public string GetCommandMessage(RequestCommand command)
        {
            string message;
            switch (command)
            {
                case RequestCommand.Accept:
                    message = string.Format(Messages.HasBeen, Labels.Accepted.ToLowerInvariant());
                    break;

                case RequestCommand.NotAccept:
                    message = string.Format(Messages.HasNotBeen, Labels.Accepted.ToLowerInvariant());
                    break;

                case RequestCommand.Approve:
                    message = string.Format(Messages.HasBeen, Labels.Approved.ToLowerInvariant());
                    break;

                case RequestCommand.Reject:
                    message = string.Format(Messages.HasBeen, Labels.Rejected.ToLowerInvariant());
                    break;

                case RequestCommand.Process:
                    message = string.Format(Messages.HasBeen, Labels.Processed.ToLowerInvariant());
                    break;

                case RequestCommand.Void:
                    message = string.Format(Messages.HasBeen, Labels.Void.ToLowerInvariant());
                    break;

                case RequestCommand.Reset:
                    message = string.Format(Messages.HasBeen, Labels.Reseted.ToLowerInvariant());
                    break;

                default:
                    message = string.Format(Messages.HasBeen, Labels.Void.ToLowerInvariant());
                    break;
            }

            return message;
        }

        private static byte GetActionResetRequest(byte statusId)
        {
            byte action;
            switch (statusId)
            {
                case (byte)CurrentRequestStatus.Verified:
                case (byte)CurrentRequestStatus.NotVerified:
                    action = (byte)SettingsSendMail.Reset1;
                    break;

                case (byte)CurrentRequestStatus.Approved:
                case (byte)CurrentRequestStatus.Rejected:
                    action = (byte)SettingsSendMail.Reset2;
                    break;

                default:
                    action = (byte)SettingsSendMail.Reset3;
                    break;
            }

            return action;
        }

        private static EmailModel GetMailAndLetstalkApprover(RequestRelatedPeopleModel relatedPeople)
        {
            var emailModel = new EmailModel();
            SupervisorNameModel approver = relatedPeople.Approver;
            if (approver != null && relatedPeople.EmailSetting.AllowSendApprover)
            {
                emailModel.SendTo = approver.Email;
                emailModel.SendToNickName = approver.NickName;
                emailModel.LetsTalk = !string.IsNullOrWhiteSpace(approver.LetsTalk) ? approver.LetsTalk : string.Empty;
            }

            return emailModel;
        }

        private static void GetMailAndLetstalkVerifier(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            bool isValidVerifier = relatedPeople.Verifier != null;
            bool isValidApprover = relatedPeople.Approver != null;

            if (relatedPeople.EmailSetting.AllowSendVerifier && isValidVerifier)
            {
                if (string.IsNullOrWhiteSpace(emailModel.SendTo))
                {
                    emailModel.SendTo = relatedPeople.Verifier.Email;
                    emailModel.SendToNickName = relatedPeople.Verifier.NickName;
                }
                else
                {
                    bool isValidEmail = isValidApprover
                        && !string.IsNullOrWhiteSpace(relatedPeople.Verifier.Email)
                        && !relatedPeople.Verifier.Email.Equals(relatedPeople.Approver.Email, StringComparison.OrdinalIgnoreCase);
                    if (isValidEmail)
                    {
                        emailModel.SendTo += ";" + relatedPeople.Verifier.Email;
                    }
                }

                GetLetstalkVerifier(relatedPeople, emailModel);
            }
        }

        private static void GetLetstalkVerifier(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (string.IsNullOrWhiteSpace(emailModel.LetsTalk))
            {
                emailModel.LetsTalk = relatedPeople.Verifier.LetsTalk;
            }
            else
            {
                bool isValidLetalk = relatedPeople.Approver != null
                       && !string.IsNullOrWhiteSpace(relatedPeople.Verifier.LetsTalk)
                       && !relatedPeople.Verifier.LetsTalk.Equals(relatedPeople.Approver.LetsTalk, StringComparison.OrdinalIgnoreCase);
                if (isValidLetalk)
                {
                    emailModel.LetsTalk += ";" + relatedPeople.Verifier.LetsTalk;
                }
            }
        }

        private static void GetMailAndLetstalkHR(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (relatedPeople.EmailSetting.AllowSendHR && relatedPeople.ListHR != null && relatedPeople.ListHR.Any())
            {
                var mailHRs = string.Join(";", relatedPeople.ListHR.Where(x => !string.IsNullOrWhiteSpace(x.Email)).Select(x => x.Email).ToList());
                var letstalks = string.Join(";", relatedPeople.ListHR.Where(x => !string.IsNullOrWhiteSpace(x.LetsTalk)).Select(x => x.LetsTalk).ToList());

                if (string.IsNullOrWhiteSpace(emailModel.SendTo))
                {
                    emailModel.SendTo = mailHRs;
                    emailModel.SendToNickName = Labels.HR;
                }
                else
                {
                    emailModel.SendCC = mailHRs;
                }

                if (string.IsNullOrWhiteSpace(emailModel.LetsTalk))
                {
                    emailModel.LetsTalk = letstalks;
                }
                else
                {
                    emailModel.LetsTalk += ";" + letstalks;
                }
            }
        }

        private static void GetMailAndLetstalkOwnerRequest(RequestRelatedPeopleModel relatedPeople, EmailModel emailModel)
        {
            if (relatedPeople.EmailSetting.AllowSendUser && relatedPeople.Requester != null)
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
}