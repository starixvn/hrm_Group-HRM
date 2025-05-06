using System;
using HRM.Core;
using Microsoft.AspNetCore.Http;

namespace HRM.Models.Payroll;

public class UploadHistory
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string SavedFileName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime Month { get; set; }
    public short PeriodType { get; set; }
    public string NickName { get; set; }

    public string MonthFormatted => Month.ToString("MM-yyyy");
    public string ClientCreatedTime => CreatedDate.ToClientTimeZone().ToString("MM/dd/yyyy HH:mm:ss");
}
