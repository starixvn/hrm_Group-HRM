namespace HRM.Core;

using System;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

public static class HSSFSheetExtensions
{
    public static ISheet CloneSheet(this IWorkbook workbook, string sourceSheetName, string newSheetName)
    {
        var sheetIndex = workbook.GetSheetIndex(sourceSheetName);
        var newSheet = workbook.CloneSheet(sheetIndex);
        var newSheetIndex = workbook.GetSheetIndex(newSheet);
        workbook.SetSheetName(newSheetIndex, newSheetName);
        return newSheet;
    }

    public static void RemoveSheet(this IWorkbook workbook, string sheetName)
    {
        var sheet = workbook.GetSheet(sheetName);

        if (sheet != null)
        {
            var sheetIndex = workbook.GetSheetIndex(sheet);
            workbook.RemoveSheetAt(sheetIndex);
        }
    }

    public static IRow CopyRow(this ISheet sheet, ISheet sourceSheet, int sourceRowIndex, int destRowIndex)
    {
        if (sheet.LastRowNum < destRowIndex)
        {
            for (var i = sheet.LastRowNum + 1; i <= destRowIndex; i++)
            {
                sheet.CreateRow(i);
            }
        }

        // Get the source / new row
        var sourceRow = sourceSheet.GetRow(sourceRowIndex);
        if (sourceRow == null)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
        }

        var newRow = sheet.GetRow(destRowIndex) ?? sheet.CreateRow(destRowIndex);

        // Loop through source columns to add to new row
        for (var i = 0; i < sourceRow.LastCellNum; i++)
        {
            // Grab a copy of the old/new cell
            var oldCell = sourceRow.GetCell(i);
            var newCell = newRow.CreateCell(i);

            CopyCell(oldCell, newCell);
        }

        return newRow;
    }

    public static ICell CopyCell(ICell sourceCell, ICell destCell)
    {
        if (sourceCell == null)
        {
            return null;
        }

        destCell.CellStyle = sourceCell.CellStyle;
        //// If there is a cell comment, copy
        if (destCell.CellComment != null)
        {
            destCell.CellComment = sourceCell.CellComment;
        }

        //// If there is a cell hyperlink, copy
        if (sourceCell.Hyperlink != null)
        {
            destCell.Hyperlink = sourceCell.Hyperlink;
        }

        //// Set the cell data type
        destCell.SetCellType(sourceCell.CellType);
        switch (sourceCell.CellType)
        {
            case CellType.Blank:
                destCell.SetCellValue(sourceCell.StringCellValue);
                break;

            case CellType.Boolean:
                destCell.SetCellValue(sourceCell.BooleanCellValue);
                break;

            case CellType.Error:
                destCell.SetCellErrorValue(sourceCell.ErrorCellValue);
                break;

            case CellType.Numeric:
                destCell.SetCellValue(sourceCell.NumericCellValue);
                break;

            case CellType.String:
                destCell.SetCellValue(sourceCell.RichStringCellValue);
                break;

            case CellType.Unknown:
                destCell.SetCellValue(sourceCell.StringCellValue);
                break;

            default:
                destCell.SetCellValue(0);
                break;
        }

        return destCell;
    }

    public static int MergeCells(this ISheet sheet, int firstRow, int firstCol, int lastRow, int lastCol)
    {
        var rangeAddress = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
        return sheet.AddMergedRegion(rangeAddress);
    }

    public static void SetCellValue(this ICell cell, decimal value)
    {
        cell.SetCellValue(Convert.ToDouble(value));
    }

    public static void SetCellValue(this ICell cell, object value)
    {
        if (value != null)
        {
            var valueType = value.GetType();

            if (valueType == typeof(bool))
            {
                cell.SetCellValue(Convert.ToBoolean(value));
                return;
            }

            if (valueType == typeof(DateTime))
            {
                cell.SetCellValue(Convert.ToDateTime(value));
                return;
            }

            if (valueType == typeof(decimal))
            {
                cell.SetCellValue(Convert.ToDecimal(value));
                return;
            }

            if (valueType == typeof(double))
            {
                cell.SetCellValue(Convert.ToDouble(value));
                return;
            }

            if (valueType == typeof(int))
            {
                cell.SetCellValue(Convert.ToInt32(value));
                return;
            }

            if (valueType == typeof(long))
            {
                cell.SetCellValue(Convert.ToInt64(value));
                return;
            }

            if (valueType == typeof(string))
            {
                cell.SetCellValue(Convert.ToString(value));
            }
        }
    }
}