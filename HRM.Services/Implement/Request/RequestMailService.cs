namespace HRM.Services
{
    using System;
    using System.Linq;
    using HRM.Core;
    using HRM.Core.Enums;
    using HRM.Criteria.Request;
    using HRM.Models;
    using HRM.Models.Request;
    using HRM.Repositories;
    using HRM.Resources;

    public class RequestMailService(IRequestRepository requestRepository) : IRequestMailService
    {
        #region constant

        private const string _MailContentLeave = "{0} to {1} ({2} days)";
        private const string _MailContentLeaveOneDay = "{0}";
        private const string _MailContentLeaveHalfDay = "{0} (0.5 day from {1} to {2})";

        #endregion constant

        public EmailModel GetMailToMailCCAndLetstalk(long requestId, int action)
        {
            var criteria = new GetRequestRelatedPeopleCriteria
            {
                RequestId = requestId,
                Action = action
            };
            var relatedPeople = requestRepository.GetRequestRelatedPeople(criteria);

            var emailModel = GetMailAndLetstalkApprover(relatedPeople);

            GetMailAndLetstalkVerifier(relatedPeople, emailModel);

            RelatedPeopleAllowSendHR(relatedPeople);
            GetMailAndLetstalkHR(relatedPeople, emailModel);

            GetMailAndLetstalkOwnerRequest(relatedPeople, emailModel);

            if (relatedPeople.EmailSetting.AllowSendMailGroup)
            {
                GetMailGroup(relatedPeople, emailModel);
            }

            emailModel.IsAllowSendRequester = relatedPeople.EmailSetting.AllowSendUser;

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

        private static EmailModel GetMailAndLetstalkApprover(RequestRelatedPeopleModel relatedPeople)
        {
            var emailModel = new EmailModel();
            var approver = relatedPeople.Approver;
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
            var isValidVerifier = relatedPeople.Verifier != null;
            var isValidApprover = relatedPeople.Approver != null;

            if (relatedPeople.EmailSetting.AllowSendVerifier && isValidVerifier)
            {
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
                var isValidLetalk = relatedPeople.Approver != null
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