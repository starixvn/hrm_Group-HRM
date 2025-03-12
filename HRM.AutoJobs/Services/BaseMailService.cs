namespace HRM.AutoJobs.Services;

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Criteria.MailDeliver;
using Models;
using Models.MailDeliver;
using MailKit.Net.Smtp;
using MimeKit;

public class BaseMailService(
    AppSettings appSettings,
    IBaseService baseService) : IBaseMailService
{
    public async Task<bool> SendEmailAsync(MailRecord email)
    {
        var isSucessfull = false;
        if (appSettings.EnableSendMail && email != null && IsValidEmail(email.MailTo))
        {
            email.MailSetting = await GetMailSettingByIdAsync(appSettings.AppMailId);
            await InsertMailRecordAsync(email);
            isSucessfull = await SendAsync(email);
        }

        return isSucessfull;
    }

    private async Task<bool> SendAsync(MailRecord email)
    {
        var isSucessfull = email.MailSetting.Port == 465 ? await SendByMailKitAsync(email) : await SendByMailNetAsync(email);
        return isSucessfull;
    }

    private async Task<bool> SendByMailKitAsync(MailRecord email)
    {
        MimeMessage message = GetMessageContent(email);
        var isSucessfull = false;

        using (var mailclient = new MailKit.Net.Smtp.SmtpClient())
        {
            mailclient.Connect(email.MailSetting.Server, email.MailSetting.Port, email.MailSetting.IsAuth);
            if (email.MailSetting.IsAuth)
            {
                mailclient.Authenticate(email.MailSetting.Username, email.MailSetting.Password);
            }

            try
            {
                mailclient.Send(message);
                isSucessfull = true;
                await UpdateMailStatusAsync(email, isSucessfull);
            }
            catch (AuthenticationException ex)
            {
                await HandleMailErrorAsync(email, ex.Message, ex.StackTrace);
                throw;
            }
            catch (SmtpCommandException ex)
            {
                await HandleMailErrorAsync(email, ex.Message, ex.StackTrace);
                throw;
            }
            catch (SmtpProtocolException ex)
            {
                await HandleMailErrorAsync(email, ex.Message, ex.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                await HandleMailErrorAsync(email, ex.Message, ex.StackTrace);
                throw;
            }

            await mailclient.DisconnectAsync(true);
        }

        return isSucessfull;
    }

    private async Task<bool> SendByMailNetAsync(MailRecord email)
    {
        var isSucessfull = false;
        try
        {
            var msg = new MailMessage
            {
                From = new MailAddress(email.MailSetting.Username)
            };
            if (!string.IsNullOrEmpty(email.MailTo))
            {
                GetMailFromString(email.MailTo, msg.To);
                GetMailFromString(email.MailCc, msg.CC);
                GetMailFromString(email.MailBcc, msg.Bcc);

                msg.Subject = email.Subject;
                msg.SubjectEncoding = Encoding.UTF8;
                msg.IsBodyHtml = true;
                msg.Body = email.Body;
                msg.BodyEncoding = Encoding.UTF8;
                msg.Priority = MailPriority.Normal;

                var client = new System.Net.Mail.SmtpClient
                {
                    Port = email.MailSetting.Port,
                    Host = email.MailSetting.Server
                };
                if (email.MailSetting.IsAuth)
                {
                    client.Credentials = new NetworkCredential(email.MailSetting.Username, email.MailSetting.Password);
                    client.EnableSsl = true;
                }

                ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidationCallback;

                client.SendCompleted += SendCompletedCallback;
                client.SendAsync(msg, email);
                isSucessfull = true;
            }
        }
        catch (Exception ex)
        {
            await HandleMailErrorAsync(email, ex.Message, ex.StackTrace);
            throw;
        }

        return isSucessfull;
    }

    private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        var email = (MailRecord)e.UserState;
        bool status = e.Error == null;
        if (!status)
        {
            HandleMailErrorAsync(email, (e.Error.InnerException != null ? e.Error.InnerException.Message : e.Error.Message), e.Error.StackTrace).Wait();
        }
        else
        {
            UpdateMailStatusAsync(email, true).Wait();
        }
    }

    private static void GetMailFromString(string mailString, MailAddressCollection mails)
    {
        if (!string.IsNullOrWhiteSpace(mailString))
        {
            var emails = RemoveDoubleComma(mailString);
            var mailArrs = emails.Split(';');
            foreach (var mail in mailArrs)
            {
                if (!string.IsNullOrEmpty(mail))
                {
                    mails.Add(mail);
                }
            }
        }
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
        message.From.Add(new MailboxAddress(string.Empty, email.MailSetting.Username));
        GetMailAddressFromString(email.MailTo, message.To);
        GetMailAddressFromString(email.MailCc, message.Cc);
        GetMailAddressFromString(email.MailBcc, message.Bcc);

        message.Subject = email.Subject;
        message.Priority = (MessagePriority)email.Priority;

        var builder = new BodyBuilder
        {
            HtmlBody = email.Body
        };
        foreach (MimeEntity attachment in email.Attachments)
        {
            builder.Attachments.Add(attachment);
        }

        message.Body = builder.ToMessageBody();

        return message;
    }

    private async Task<MailSetting> GetMailSettingByIdAsync(int appId)
    {
        var criteria = new GetMailSettingByIdCriteria
        {
            SettingId = appId
        };
        return await baseService.GetDataEntityAsync<MailSetting>(criteria);
    }

    private async Task InsertMailRecordAsync(MailRecord email)
    {
        var criteria = new InsertMailRecordCommand
        {
            AppId = email.MailSetting.AppId,
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
        var command = new InsertMailErrorCommand
        {
            AppId = email.MailSetting.AppId,
            MailId = email.MailId,
            Message = errorMessage,
            StackTrace = stackTrace,
            ErrorTime = DateTimeOffset.Now.DateTime
        };

        await baseService.ExecuteNonQueryAsync(command);
    }

    private async Task UpdateMailStatusAsync(MailRecord email, bool isSucessfull)
    {
        var command = new UpdateMailStatusCommand
        {
            MailId = email.MailId,
            Status = isSucessfull,
            MailUpdatedDate = DateTimeOffset.Now.DateTime
        };

        await baseService.ExecuteNonQueryAsync(command);
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        return regex.IsMatch(email);
    }

    private static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        => true;
}