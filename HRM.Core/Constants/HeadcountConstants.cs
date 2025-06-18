using System.Drawing;

namespace HRM.Core.Constants;

public static class HeadcountConstants
{
    public const int StartColumn = 3; // C column
    public const int PropertyNumber = 4;
    public const int TotalMonths = 12;

    public const string GrandTotalLabel = "GRAND TOTAL";
    public const string TotalLabel = "TOTAL";

    public static class Colors
    {
        public static readonly Color MidnightBlue = Color.MidnightBlue;
        public static readonly Color LightBlue = Color.FromArgb(217, 231, 253);
        public static readonly Color GrandTotalBlue = Color.FromArgb(164, 194, 244);
        public static readonly Color White = Color.White;
        public static readonly Color LightGray = Color.FromArgb(235, 237, 236);
        public static readonly Color DarkGray = Color.DarkGray;
    }

    public static class PropertyNames
    {
        public const string Label1 = "Label1";
        public const string Label2 = "Label2";
        public const string Label3 = "Label3";
    }
}