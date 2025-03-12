namespace HRM.Models.Request
{
    using HRM.Core.Enums;
    using Resources;

    public class RequestStatus : EntityBase<byte>
    {
        public new string Name
        {
            get
            {
                return Id switch
                {
                    (byte)CurrentRequestStatus.Pending => Labels.Pending,
                    (byte)CurrentRequestStatus.Verified => Labels.Verified,
                    (byte)CurrentRequestStatus.NotVerified => Labels.NotVerified,
                    (byte)CurrentRequestStatus.Approved => Labels.Approved,
                    (byte)CurrentRequestStatus.Rejected => Labels.Rejected,
                    (byte)CurrentRequestStatus.Processed => Labels.Processed,
                    (byte)CurrentRequestStatus.Void => Labels.Void,
                    _ => string.Empty
                };
            }
        }

        public override string ToString() => Name;
    }
}