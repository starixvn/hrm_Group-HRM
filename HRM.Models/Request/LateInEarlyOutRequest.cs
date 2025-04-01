namespace HRM.Models.Request;

using System;
using Core.Enums;
using Resources;

public class LateInEarlyOutRequest : BaseRequest
{
    public DateTime OnDate { get; set; }

    public int LateInMins { get; set; }

    public int EarlyOutMins { get; set; }

    public int ActualLateInEarlyOut { get; set; }

    public string ShiftName { get; set; }

    public byte ReasonId { get; set; }

    public string ReasonName
    {
        get
        {
            return ReasonId switch
            {
                (byte)LateInEarlyOutReason.ManagementAllow => Labels.ManagementAllow,
                (byte)LateInEarlyOutReason.Other => Labels.Other,
                _ => string.Empty
            };
        }
    }
}