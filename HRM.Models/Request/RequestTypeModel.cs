namespace HRM.Models.Request
{
    using Core;
    using Core.Enums;

    public class RequestTypeModel : EntityBase<short>
    {
        public new string Name => Id.RequestTypeToText();

        public int FilterRequest
        {
            get
            {
                return Id switch
                {
                    (byte)RequestType.CheckIn or (byte)RequestType.CheckOut => (byte)RequestFilter.CheckInOut,
                    (byte)RequestType.LateIn or (byte)RequestType.EarlyOut => (byte)RequestFilter.LateInEarlyOut,
                    (byte)RequestType.ChangeShift => (byte)RequestFilter.ChangeShift,
                    (byte)RequestType.Leave => (byte)RequestFilter.Leave,
                    (byte)RequestType.KeepInView => (byte)RequestFilter.KeepInView,
                    _ => (byte)RequestFilter.All
                };
            }
        }

        public int FilterRequestHistory
        {
            get
            {
                return Id switch
                {
                    (byte)RequestType.CheckIn or (byte)RequestType.CheckOut => (byte)RequestFilter.CheckInOut,
                    (byte)RequestType.LateIn or (byte)RequestType.EarlyOut => (byte)RequestFilter.LateInEarlyOut,
                    (byte)RequestType.ChangeShift => (byte)RequestFilter.ChangeShift,
                    (byte)RequestType.Leave => (byte)RequestFilter.Leave,
                    (byte)RequestType.KeepInView => (byte)RequestFilter.KeepInView,
                    _ => (byte)RequestFilter.All
                };
            }
        }

        public override string ToString() => Name;
    }
}