namespace HRM.Models.Message;

using Core;
using Core.Enums;
using System;

public class Message
{
    private DateTime serverCreatedDate;
    public int MessageId { get; set; }

    public string Subject { get; set; }

    public string Contents { get; set; }

    public int Category { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public DateTime CreatedDate
    {
        get => serverCreatedDate;
        set => serverCreatedDate = value.ToClientTimeZone();
    }

    public string SendToEmail { get; set; }

    public string SendTo { get; set; }

    public bool HasSendEmail { get; set; }

    public string EmployeeIds { get; set; }

    public string TeamIds { get; set; }

    public bool IsRead { get; set; }

    public string CategoryName
    {
        get
        {
	        var category = Category switch
	        {
		        (int)MessageCategory.Notification => MessageCategory.Notification.ToString(),
		        (int)MessageCategory.Warning => MessageCategory.Warning.ToString(),
		        (int)MessageCategory.Violation => MessageCategory.Violation.ToString(),
		        _ => string.Empty
	        };

	        return category;
        }
    }
}