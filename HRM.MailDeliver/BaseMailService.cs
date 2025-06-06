namespace HRM.MailDeliver;

using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Core;
using Core.Services;
using Criteria;
using MailKit.Net.Smtp;
using MimeKit;

public class BaseMailService(IBaseService baseService) : IBaseMailService
{
    public async Task<bool> SendEmailAsync(MailRecord email)
    {
        var isSuccessful = false;
        if (email != null && Utilities.IsValidEmail(email.MailTo))
        {
            email.MailConfig = await GetMailConfigByIdAsync(email.AppMailId);
            await InsertMailRecordAsync(email);
            isSuccessful = await SendByMailKitAsync(email);
        }

        return isSuccessful;
    }

    private async Task<bool> SendByMailKitAsync(MailRecord email)
    {
         var isSuccessful = false;
        var message = GetMessageContent(email);
        using var mailClient = new SmtpClient();
       
        mailClient.ServerCertificateValidationCallback += RemoteCertificateValidationCallback;

        try
        {
            await mailClient.ConnectAsync(email.MailConfig.Server, email.MailConfig.Port, email.MailConfig.IsAuth);
            if (email.MailConfig.IsAuth)
            {
                await mailClient.AuthenticateAsync(email.MailConfig.Username, email.MailConfig.Password);
            }
            await mailClient.SendAsync(message);
            isSuccessful = true;
            await UpdateMailStatusAsync(email, true);
        }
        catch (Exception ex)
        {
            await HandleMailErrorAsync(email, ex.Message, ex.StackTrace);
        }

        await mailClient.DisconnectAsync(true);

        return isSuccessful;
    }

    private static void GetMailAddressFromString(string mailString, InternetAddressList mails)
    {
        if (!string.IsNullOrWhiteSpace(mailString))
        {
            var emails = RemoveDoubleComma(mailString);
            var mailArrays = emails.Split(';');
            foreach (var mail in mailArrays)
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

    private async Task UpdateMailStatusAsync(MailRecord email, bool isSuccessful)
    {
        var criteria = new UpdateMailStatusCriteria
        {
            MailId = email.MailId,
            Status = isSuccessful,
            MailUpdatedDate = DateTimeOffset.Now.DateTime
        };

        await baseService.ExecuteNonQueryAsync(criteria);
    }

    private static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        => true;
}