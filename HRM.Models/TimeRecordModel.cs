namespace HRM.Models;

using System;
using Core;
using Resources;

public class TimeRecordModel
{
    private DateTime serverRecordTime;

    public int RecordTimeId { get; set; }
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }
    public string Team { get; set; }

    public DateTime RecordTime
    {
        get => serverRecordTime;
        set => serverRecordTime = value.ToClientTimeZone();
    }

    public byte? InOutType { get; set; }

    public string InOutName
    {
        get
        {
            var typeName = InOutType switch
            {
                0 => Labels.In,
                1 => Labels.Out,
                _ => Labels.None
            };

            return typeName;
        }
    }

    public string SectionCode { get; set; }

    public string DepartmentCode { get; set; }

    public bool IsCheckInOutOnline { get; set; }
}