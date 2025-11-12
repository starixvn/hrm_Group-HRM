using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Models.Report.HeadcountReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace HRM.Services.Helper;

public static class HeadcountReportHelper
{
    public static void WriteDataToCell(this ExcelRange cell, dynamic value, Color backgroundColor, bool isBold = false)
    {
        cell.Value = value ?? 0;
        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, HeadcountConstants.Colors.DarkGray);

        var fontColor = GetContrastingFontColor(backgroundColor);
        cell.Style.Font.Color.SetColor(fontColor);

        cell.Style.WrapText = true;
        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
        cell.Style.Fill.BackgroundColor.SetColor(backgroundColor);
        cell.Style.Font.Bold = isBold;
        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    }

    public static void WriteDataToCell(this ExcelRange cell, dynamic value, bool isBold = false)
    {
        WriteDataToCell(cell, value, Color.White, isBold);
    }

    public static void WriteHeader(this ExcelRange cell, dynamic value)
    {

        WriteDataToCell(cell, value, HeadcountConstants.Colors.LightBlue, true);
    }

    private static Color GetContrastingFontColor(Color backgroundColor)
    {
        var luminance = (0.299 * backgroundColor.R + 0.587 * backgroundColor.G + 0.114 * backgroundColor.B) / 255; // Calculate luminance to determine if background is dark or light
        return luminance > 0.5 ? Color.Black : Color.White; // Use white text for dark backgrounds, black text for light backgrounds
    }

    public static int GetMaxMonth(int year)
    {
        var currentDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;

        if (year < currentDate.Year)
        {
            return HeadcountConstants.TotalMonths;
        }

        if (year == currentDate.Year)
        {
            return currentDate.Month;
        }

        return 0;
    }

    public static IEnumerable<UnitReport> FilterValidUnitReports(this IEnumerable<UnitReport> source) => source.Where(IsValidUnitReport);

    public static bool IsValidUnitReport(this UnitReport report) =>
        report is { Details: not null } &&
        report.Details.Any() &&
        report.Details.Any(d => d.NewHire > 0 || d.ProbationResign > 0 || d.ConfirmedResign > 0 || d.CurrentHeadCount > 0);

    public static void AddUnitReport(this List<UnitReport> list, UnitReport report)
    {
        if (IsValidUnitReport(report))
        {
            list.Add(report);
        }
    }

    public static void AddRangeUnitReport(this List<UnitReport> list, IEnumerable<UnitReport> reports)
    {
        if (reports != null)
        {
            list.AddRange(reports.Where(IsValidUnitReport));
        }
    }

    public static IOrderedEnumerable<IGrouping<TKey, TSource>> GroupByAndOrderByKey<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector) where TKey : IComparable<TKey>
    {
        return source.GroupBy(keySelector).OrderBy(g => g.Key);
    }

    public static string ExtractCategoryName(string label)
    {
        const string TotalSuffix = " TOTAL";
        if (string.IsNullOrEmpty(label))
        {
            return string.Empty;
        }

        var text = label.Replace(TotalSuffix, string.Empty);
        var index = text.IndexOf('(');
        return index > 0 ? text[..index].Trim() : text.Trim();
    }
}