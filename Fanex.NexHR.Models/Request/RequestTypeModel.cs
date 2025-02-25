namespace Fanex.NexHR.Models.Request
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;

    public class RequestTypeModel : EntityBase<short>
    {
        public new string Name
        {
            get 
            {
                return Id.RequestTypeToText();
            }
        }

        public int FilterRequest
        {
            get 
            {
                switch (Id)
                {
                    case (byte)RequestType.CheckIn:
                    case (byte)RequestType.CheckOut:
                        return (byte)RequestFilter.CheckInOut;
                    case (byte)RequestType.LateIn:
                    case (byte)RequestType.EarlyOut:
                        return (byte)RequestFilter.LateInEarlyOut;
                    case (byte)RequestType.ChangeShift:
                        return (byte)RequestFilter.ChangeShift;
                    case (byte)RequestType.Leave:
                        return (byte)RequestFilter.Leave;
                    case (byte)RequestType.KeepInView:
                        return (byte)RequestFilter.KeepInView;
                    default:
                        return (byte)RequestFilter.All;
                }
            }
        }

        public int FilterRequestHistory
        {
            get
            {
                switch (Id)
                {
                    case (byte)RequestType.CheckIn:
                    case (byte)RequestType.CheckOut:
                        return (byte)RequestFilter.CheckInOut;
                    case (byte)RequestType.LateIn:
                    case (byte)RequestType.EarlyOut:
                        return (byte)RequestFilter.LateInEarlyOut;
                    case (byte)RequestType.ChangeShift:
                        return (byte)RequestFilter.ChangeShift;
                    case (byte)RequestType.Leave:
                        return (byte)RequestFilter.Leave;
                    case (byte)RequestType.KeepInView:
                        return (byte)RequestFilter.KeepInView;
                    default:
                        return (byte)RequestFilter.All;
                }
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
