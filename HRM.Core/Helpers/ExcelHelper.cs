using HRM.Core.Constants;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace HRM.Core.Helpers;

public static class ExcelHelper
{
    public static void WriteDataToCell(this ExcelRange cell, dynamic value, Color backgroundColor, bool isBold = false)
    {
        cell.Value = value ?? string.Empty;
        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, HeadcountConstants.Colors.DarkGray);

        var fontColor = GetContrastingFontColor(backgroundColor);
        cell.Style.Font.Color.SetColor(fontColor);

        cell.Style.WrapText = true;
        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
        cell.Style.Fill.BackgroundColor.SetColor(backgroundColor);
        cell.Style.Font.Bold = isBold;
    }

    private static Color GetContrastingFontColor(Color backgroundColor)
    {
        var luminance = (0.299 * backgroundColor.R + 0.587 * backgroundColor.G + 0.114 * backgroundColor.B) / 255; // Calculate luminance to determine if background is dark or light
        return luminance > 0.5 ? Color.Black : Color.White; // Use white text for dark backgrounds, black text for light backgrounds
    }
}