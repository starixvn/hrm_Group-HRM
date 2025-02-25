namespace Fanex.NexHR.Models.Request
{
    using Fanex.NexHR.Core.Enums;
    using Resources;

    public class RequestStatus : EntityBase<byte>
    {
        public new string Name
        {
            get
            {
                switch (Id)
                {
                    case (byte)CurrentRequestStatus.Pending:
                        return Labels.Pending;

                    case (byte)CurrentRequestStatus.Verified:
                        return Labels.Verified;

                    case (byte)CurrentRequestStatus.NotVerified:
                        return Labels.NotVerified;

                    case (byte)CurrentRequestStatus.Approved:
                        return Labels.Approved;

                    case (byte)CurrentRequestStatus.Rejected:
                        return Labels.Rejected;

                    case (byte)CurrentRequestStatus.Processed:
                        return Labels.Processed;

                    case (byte)CurrentRequestStatus.Void:
                        return Labels.Void;

                    default:
                        return string.Empty;
                }
            }
        }

        public override string ToString() => Name;
    }
}