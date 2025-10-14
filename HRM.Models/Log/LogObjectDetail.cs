namespace HRM.Models.Log;

using Core;
using System;
using System.Collections.Generic;

public class LogObjectDetail
{
    private DateTime serverLogDate;
    public DateTime LogDate
    {
        get => serverLogDate;
        set => serverLogDate = value.ToClientTimeZone();
    }
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string Nickname { get; set; }
    public string OldData { get; set; }
    public string Data { get; set; }

    public string Field { get; set; }

    public IList<FieldValue> Fields { get; set; } = new List<FieldValue>();
}