namespace HRM.Models.Request;

using System;
using System.Collections.Generic;
using Core.Enums;
using Resources;

public class BaseRequest
{
    public long RequestId { get; set; }
    public byte RequestTypeId { get; set; }

    public string Remark { get; set; }

    public Requester Requester { get; set; }
    public Requester Verifier { get; set; }
    public Requester Approver { get; set; }
    public Requester ApprovedBy { get; set; }
    public Requester HR { get; set; }
    public Requester Modifier { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime VerifiedDate { get; set; }
    public DateTime ApprovedDate { get; set; }
    public DateTime ProcessedDate { get; set; }

    public DateTime InsertedDate { get; set; }

    public byte CurrentStatus { get; set; }
    public byte VerifiedStatus { get; set; }
    public byte ApprovedStatus { get; set; }
    public byte ProcessedStatus { get; set; }

    public RequestStatus Status => new()
    {
        Id = CurrentStatus
    };

    public string RefNO { get; set; }

    public string RequestTypeName
    {
        get
        {
            var requestTypeName = RequestTypeId switch
            {
                (byte)RequestType.CheckIn => Labels.CheckIn,
                (byte)RequestType.CheckOut => Labels.CheckOut,
                (byte)RequestType.LateIn => Labels.LateIn,
                (byte)RequestType.EarlyOut => Labels.EarlyOut,
                (byte)RequestType.ChangeShift => Labels.ChangeShift,
                (byte)RequestType.Leave => Labels.Leave,
                (byte)RequestType.KeepInView => Labels.CompensationLeave,
                (byte)RequestType.Overtime => Labels.Overtime,
                _ => string.Empty
            };

            return requestTypeName;
        }
    }

    public IList<AttachFile> AttachFiles { get; set; }
}