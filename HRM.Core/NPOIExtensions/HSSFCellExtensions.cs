namespace HRM.Core.NPOIExtensions;

using System;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

public static class HSSFCellExtensions
{
    public static void SetCellValue(this ICell cell, object value)
    {
        if (value == null)
        {
            cell.SetCellValue(string.Empty);
        }
        else
        {
            var stringValue = value as string;
            var richTextString = value as IRichTextString;

            if (!string.IsNullOrEmpty(stringValue))
            {
                cell.SetCellValue(stringValue);
            }
            else switch (value)
            {
	            case int i:
		            cell.SetCellValue(i);
		            break;
	            case long l:
		            cell.SetCellValue(l);
		            break;
	            case double d:
		            cell.SetCellValue(d);
		            break;
	            case decimal value1:
		            cell.SetCellValue(value1);
		            break;
	            case bool b:
		            cell.SetCellValue(b);
		            break;
	            default:
	            {
		            if (value is DateTime time)
		            {
			            cell.SetCellValue(time);
		            }
		            else if (richTextString != null)
		            {
			            cell.SetCellValue(richTextString);
		            }
		            else
		            {
			            cell.SetCellValue(string.Empty);
		            }

		            break;
	            }
            }
        }
    }

    public static void SetCellValue(this ICell cell, double? value)
    {
        var result = value ?? 0;

        cell.SetCellValue(result);
    }

    public static void SetCellValue(this ICell cell, decimal value)
    {
        cell.SetCellValue(Convert.ToDouble(value));
    }

    public static void SetCellValue(this ICell cell, decimal? value)
    {
        var result = value ?? 0;

        cell.SetCellValue(result);
    }

    public static void SetCellValue(this ICell cell, int value)
    {
        cell.SetCellValue(Convert.ToDouble(value));
    }

    public static void SetCellValue(this ICell cell, int? value)
    {
        var result = value ?? 0;

        cell.SetCellValue(result);
    }

    public static void SetCellValue(this ICell cell, long value)
    {
        cell.SetCellValue(Convert.ToDouble(value));
    }

    public static void SetCellValue(this ICell cell, long? value)
    {
        var result = value ?? 0;

        cell.SetCellValue(result);
    }

    public static void CopyFrom(this ICell cell, ICell sourceCell)
    {
        // Copy merged cells.
        CellRangeAddress sourceRange = null;
        var i = 0;

        while (sourceRange == null && i < sourceCell.Sheet.NumMergedRegions)
        {
            var range = sourceCell.Sheet.GetMergedRegion(i);

            if (range.FirstColumn <= sourceCell.ColumnIndex
                && range.LastColumn >= sourceCell.ColumnIndex
                && range.FirstRow <= sourceCell.RowIndex
                && range.LastRow >= sourceCell.RowIndex)
            {
                sourceRange = range;
            }

            i += 1;
        }

        if (sourceRange != null)
        {
            cell.Sheet.AddMergedRegion(sourceRange.Clone());
        }

        // Copy style
        cell.CellStyle = sourceCell.CellStyle;
    }
}