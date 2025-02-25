namespace HRM.Core
{
    using HRM.Core.Enums;
    using HRM.Resources;

    public static class Extensions
    {
        public static string RequestTypeToText(this short type)
        {
            return type switch
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
        }
    }
}