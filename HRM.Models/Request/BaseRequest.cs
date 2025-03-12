namespace HRM.Models.Request
{
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
        public Requester Creater { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime VerifiedDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime ProcessedDate { get; set; }

        public DateTime InsertedDate { get; set; }

        public byte CurrentStatus { get; set; }
        public byte VerifiedStatus { get; set; }
        public byte ApprovedStatus { get; set; }
        public byte ProcessedStatus { get; set; }

        public RequestStatus Status => new RequestStatus
        {
            Id = CurrentStatus
        };

        public string RefNO { get; set; }

        public string RequestTypeName
        {
            get
            {
                var requestTypeName = string.Empty;
                switch (RequestTypeId)
                {
                    case (byte)RequestType.CheckIn:
                        requestTypeName = Labels.CheckIn;
                        break;

                    case (byte)RequestType.CheckOut:
                        requestTypeName = Labels.CheckOut;
                        break;

                    case (byte)RequestType.LateIn:
                        requestTypeName = Labels.LateIn;
                        break;

                    case (byte)RequestType.EarlyOut:
                        requestTypeName = Labels.EarlyOut;
                        break;

                    case (byte)RequestType.ChangeShift:
                        requestTypeName = Labels.ChangeShift;
                        break;

                    case (byte)RequestType.Leave:
                        requestTypeName = Labels.Leave;
                        break;

                    case (byte)RequestType.KeepInView:
                        requestTypeName = Labels.CompensationLeave;
                        break;

                    case (byte)RequestType.Overtime:
                        requestTypeName = Labels.Overtime;
                        break;

                    default:
                        requestTypeName = string.Empty;
                        break;
                }

                return requestTypeName;
            }
        }

        public IList<AttachFile> AttachFiles { get; set; }
    }
}