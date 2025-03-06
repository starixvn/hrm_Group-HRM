namespace HRM.MailDeliver;

using System;
using System.Threading.Tasks;
using Criteria;
using HRM.Core;
using HRM.Core.Models;
using HRM.Core.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

public class BaseMailService(AppSettings appSettings, IBaseService baseService) : IBaseMailService
{
    public async Task<bool> SendEmailAsync(MailRecord email)
    {
        var isSucessfull = false;
        if (email != null && Utilities.IsValidEmail(email.MailTo))
        {
            email.MailConfig = await GetMailConfigByIdAsync(appSettings.AppMailId);
            await InsertMailRecordAsync(email);
            isSucessfull = await SendByMailKitAsync(email);
        }

        return isSucessfull;
    }

    private async Task<bool> SendByMailKitAsync(MailRecord email)
    {
        var message = GetMessageContent(email);
        var isSucessfull = false;

        using var mailClient = new SmtpClient();
        await mailClient.ConnectAsync(email.MailConfig.Server, email.MailConfig.Port, email.MailConfig.IsAuth);
        if (email.MailConfig.IsAuth)
        {
            await mailClient.AuthenticateAsync(email.MailConfig.Username, email.MailConfig.Password);
        }

        try
        {
            await mailClient.SendAsync(message);
            isSucessfull = true;
            await UpdateMailStatusAsync(email, isSucessfull);
        }
        catch (AuthenticationException ex)
        {
            await HandleMailErrorAsync(email, ex.Message, ex.StackTrace);
        }
        catch (SmtpCommandException ex)
        {
            await HandleMailErrorAsync(email, ex.Message, ex.StackTrace);
        }
        catch (SmtpProtocolException ex)
        {
            await HandleMailErrorAsync(email, ex.Message, ex.StackTrace);
        }
        catch (Exception ex)
        {
            await HandleMailErrorAsync(email, ex.Message, ex.StackTrace);
        }

        await mailClient.DisconnectAsync(true);

        return isSucessfull;
    }

    private static void GetMailAddressFromString(string mailString, InternetAddressList mails)
    {
        if (!string.IsNullOrWhiteSpace(mailString))
        {
            var emails = RemoveDoubleComma(mailString);
            var mailArrs = emails.Split(';');
            foreach (var mail in mailArrs)
            {
                if (!string.IsNullOrEmpty(mail))
                {
                    mails.Add(new MailboxAddress(string.Empty, mail));
                }
            }
        }
    }

    private static string RemoveDoubleComma(string mails)
    {
        var emails = mails;
        while (emails.Contains(",,"))
        {
            emails = emails.Replace(",,", ",");
            emails = emails.Replace(",", ";");
        }

        while (emails.Contains(";;"))
        {
            emails = emails.Replace(";;", ";");
        }

        return emails.TrimEnd(',', ';');
    }

    private static MimeMessage GetMessageContent(MailRecord email)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(string.Empty, email.MailConfig.Username));
        GetMailAddressFromString(email.MailTo, message.To);
        GetMailAddressFromString(email.MailCc, message.Cc);
        GetMailAddressFromString(email.MailBcc, message.Bcc);

        message.Subject = email.Subject;
        message.Priority = (MessagePriority)email.Priority;

        var builder = new BodyBuilder
        {
            HtmlBody = email.Body
        };
        foreach (var attachment in email.Attachments)
        {
            builder.Attachments.Add(attachment);
        }

        message.Body = builder.ToMessageBody();

        return message;
    }

    private async Task<MailConfig> GetMailConfigByIdAsync(int appId)
    {
        var criteria = new GetMailConfigByIdCriteria
        {
            AppId = appId
        };

        return await baseService.GetDataEntityAsync<MailConfig>(criteria);
    }

    private async Task InsertMailRecordAsync(MailRecord email)
    {
        var criteria = new InsertMailRecordCriteria
        {
            AppId = email.MailConfig.AppId,
            Subject = email.Subject,
            Body = email.Body,
            Attachments = string.Empty,
            MailTo = email.MailTo,
            MailCc = email.MailCc,
            MailBcc = email.MailBcc,
            MailDate = DateTimeOffset.Now.DateTime
        };

        await baseService.ExecuteNonQueryAsync(criteria);
        email.MailId = criteria.MailId;
    }

    private async Task HandleMailErrorAsync(MailRecord email, string errorMessage, string stackTrace)
    {
        await InsertMailErrorAsync(email, errorMessage, stackTrace);
        await UpdateMailStatusAsync(email, false);
    }

    private async Task InsertMailErrorAsync(MailRecord email, string errorMessage, string stackTrace)
    {
        var criteria = new InsertMailErrorCriteria
        {
            AppId = email.MailConfig.AppId,
            MailId = email.MailId,
            Message = errorMessage,
            StackTrace = stackTrace,
            ErrorTime = DateTimeOffset.Now.DateTime
        };

        await baseService.ExecuteNonQueryAsync(criteria);
    }

    private async Task UpdateMailStatusAsync(MailRecord email, bool isSucessfull)
    {
        var criteria = new UpdateMailStatusCriteria
        {
            MailId = email.MailId,
            Status = isSucessfull,
            MailUpdatedDate = DateTimeOffset.Now.DateTime
        };

        await baseService.ExecuteNonQueryAsync(criteria);
    }
}