namespace HRM.Core.NPOIExtensions;

using System;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

public static class HSSFSheetExtensions
{
    public static void CopyRows(this ISheet sheet, ISheet sourceSheet, int sourceRowIndex, int rowIndex, int total)
    {
        for (var i = 0; i < total; i++)
        {
            sheet.CopyRow(sourceSheet, sourceRowIndex + i, rowIndex + i);
        }
    }

    public static void CopyRow(this ISheet sheet, ISheet sourceSheet, int sourceRowIndex, int rowIndex)
    {
        sheet.CopyRow(sourceSheet, sourceRowIndex, rowIndex, 0, int.MaxValue - 1);
    }

    public static void CopyRow(this ISheet sheet, ISheet sourceSheet, int sourceRowIndex, int rowIndex, int firstColumnIndex, int lastColumnIndex)
    {
        // Expand the sheet to satisfy the number of needed rows.
        for (var i = sheet.LastRowNum + 1; i <= rowIndex; i++)
        {
            sheet.CreateRow(i);
        }

        // Get source row that will be copied.
        var sourceRow = sourceSheet.GetRow(sourceRowIndex);
        if (sourceRow != null)
        {
            // Get the row at a specific index of the current sheet.
            var newRow = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
            newRow.Height = sourceRow.Height;

            // Get the first and last available columns of the copying area.
            var firstCellNum = Math.Min(sourceRow.FirstCellNum, firstColumnIndex);
            var lastCellNum = Math.Min(sourceRow.LastCellNum, lastColumnIndex + 1);

            // Loop through each cell of the source row to copy its data to the new row.
            for (var i = firstCellNum; i < lastCellNum; i++)
            {
                // Get the source cell.
                var sourceCell = sourceRow.GetCell(i);

                // If the source cell is null then jump to next cell
                if (sourceCell == null)
                {
                    continue;
                }

                // Create a new cell at current sheet, and clone the source cell data and style to it.
                var newCell = newRow.CreateCell(i);
                CloneCell(newCell, sourceCell);

                // Copy merged area.
                CopyMergedArea(sheet, sourceSheet, sourceCell, newCell);
            }
        }
    }

    private static void CopyMergedArea(ISheet sheet, ISheet sourceSheet, ICell sourceCell, ICell newCell)
    {
        CellRangeAddress sourceRange = null;
        var j = 0;

        while (sourceRange == null && j < sourceSheet.NumMergedRegions)
        {
            var range = sourceSheet.GetMergedRegion(j);

            if (range.FirstColumn == sourceCell.ColumnIndex && range.FirstRow == sourceCell.RowIndex)
            {
                sourceRange = range;
            }

            j += 1;
        }

        if (sourceRange != null)
        {
            var newRange = new CellRangeAddress(
                newCell.RowIndex,
                newCell.RowIndex + sourceRange.LastRow - sourceRange.FirstRow,
                newCell.ColumnIndex,
                newCell.ColumnIndex + sourceRange.LastColumn - sourceRange.FirstColumn);

            sheet.AddMergedRegion(newRange);
        }
    }

    public static void RemoveRow(this ISheet sheet, int rowIndex)
    {
        sheet?.RemoveRow(sheet.GetRow(rowIndex));
    }

    public static void RemoveColumns(this ISheet sheet, List<int> columns)
    {
        if (columns is { Count: > 0 })
        {
            columns.Sort();
            for (var i = columns.Count - 1; i >= 0; i--)
            {
                RemoveColumn(sheet, columns[i]);
            }
        }
    }

    public static void RemoveColumn(this ISheet sheet, int columnIndex)
    {
        var adjustColumn = true;
        for (var rowIndex = sheet.FirstRowNum; rowIndex <= sheet.LastRowNum; rowIndex++)
        {
            var row = sheet.GetRow(rowIndex);

            // If no row exists here then skip to next row.
            if (row != null)
            {
                // Get the last available column of current row.
                int lastColumn = row.LastCellNum;

                if (lastColumn >= columnIndex)
                {
                    AdjustCell(sheet, columnIndex, adjustColumn, row, lastColumn);
                }

                // Adjust merged region
                AdjustMergedRegion(sheet, columnIndex, rowIndex);

                adjustColumn = false;
            }
        }
    }

    private static void AdjustMergedRegion(ISheet sheet, int columnIndex, int rowIndex)
    {
        for (var i = sheet.NumMergedRegions - 1; i >= 0; i--)
        {
            var mergedRegion = sheet.GetMergedRegion(i);

            if (rowIndex == mergedRegion.FirstRow)
            {
                if (columnIndex == mergedRegion.FirstColumn
                    && columnIndex == mergedRegion.LastColumn)
                {
                    sheet.RemoveMergedRegion(i);
                }
                else if (mergedRegion.FirstColumn > columnIndex)
                {
                    mergedRegion.FirstColumn -= 1;
                    mergedRegion.LastColumn -= 1;
                }
                else if (columnIndex <= mergedRegion.LastColumn)
                {
                    mergedRegion.LastColumn -= 1;
                }
                // no action at here
            }
        }
    }

    private static void AdjustCell(ISheet sheet, int columnIndex, bool adjustColumn, IRow row, int lastColumn)
    {
        for (var currentCol = columnIndex; currentCol < lastColumn; currentCol++)
        {
            // Remove old cell.
            var oldCell = row.GetCell(currentCol);
            if (oldCell != null)
            {
                row.RemoveCell(oldCell);
            }

            // Move cell.
            var nextCell = row.GetCell(currentCol + 1);
            if (nextCell != null)
            {
                var newCell = row.CreateCell(currentCol, nextCell.CellType);
                if (adjustColumn)
                {
                    sheet.SetColumnWidth(newCell.ColumnIndex, sheet.GetColumnWidth(nextCell.ColumnIndex));
                }

                CloneCell(newCell, nextCell);
            }
        }

        // Remove last cell.
        var removeCell = row.GetCell(lastColumn);
        if (removeCell != null)
        {
            row.RemoveCell(removeCell);
        }
    }

    public static void CloneCell(this ISheet sheet, ICell newCell, ICell sourceCell)
    {
        sheet.CloneCell(newCell, sourceCell);
    }

    public static ICell GetCell(this ISheet sheet, int rowIndex, int colIndex)
    {
        ICell cell = null;

        if (sheet != null)
        {
            cell = sheet.GetRow(rowIndex).GetCell(colIndex);
        }

        return cell;
    }

    public static int MergeCells(this ISheet sheet, int firstRow, int firstCol, int lastRow, int lastCol)
    {
        var rangeAddress = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
        return sheet.AddMergedRegion(rangeAddress);
    }

    public static void RemoveMergedRegions(this ISheet sheet, int firstRow, int firstCol, int lastRow, int lastCol)
    {
        if (sheet != null)
        {
            for (var i = sheet.NumMergedRegions - 1; i >= 0; i--)
            {
                var mergedRegion = sheet.GetMergedRegion(i);
                if (mergedRegion.FirstRow >= firstRow
                    && mergedRegion.LastRow <= lastRow
                    && mergedRegion.FirstColumn >= firstCol
                    && mergedRegion.LastColumn <= lastCol)
                {
                    sheet.RemoveMergedRegion(i);
                }
            }
        }
    }

    public static ICell CreateCell(this ISheet sheet, int rowIndex, int colIndex, int rowSpan, int colSpan)
    {
        var cell = sheet.CreateCell(rowIndex, colIndex);

        if (rowSpan > 0 || colSpan > 0)
        {
            MergeCells(sheet, rowIndex, colIndex, rowIndex + rowSpan, colIndex + colSpan);
        }

        return cell;
    }

    public static ICell CreateCell(this ISheet sheet, int rowIndex, int colIndex, int rowSpan, int colSpan, ICellStyle cellStyle)
    {
        var cell = sheet.CreateCell(rowIndex, colIndex, rowSpan, colSpan);
        cell.CellStyle = cellStyle;
        return cell;
    }

    public static ICell CreateCell(this ISheet sheet, int rowIndex, int colIndex)
    {
        ICell cell = null;

        if (sheet != null)
        {
            var row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);

            cell = row.GetCell(colIndex) ?? row.CreateCell(colIndex);
        }

        return cell;
    }

    private static void CloneCell(ICell newCell, ICell sourceCell)
    {
        if (sourceCell != null)
        {
            if (sourceCell.CellComment != null)
            {
                newCell.CellComment = sourceCell.CellComment;
            }

            if (sourceCell.Hyperlink != null)
            {
                newCell.Hyperlink = sourceCell.Hyperlink;
            }

            if (sourceCell.CellStyle != null)
            {
                newCell.CellStyle = sourceCell.CellStyle;
            }

            newCell.SetCellType(sourceCell.CellType);

            SetCellValue(sourceCell, newCell);
        }
    }

    private static void SetCellValue(ICell sourceCell, ICell newCell)
    {
        switch (sourceCell.CellType)
        {
            case CellType.Boolean:
                newCell.SetCellValue(sourceCell.BooleanCellValue);
                break;

            case CellType.Numeric:
                newCell.SetCellValue(sourceCell.NumericCellValue);
                break;

            case CellType.String:
                newCell.SetCellValue(sourceCell.StringCellValue);
                break;

            case CellType.Error:
                newCell.SetCellValue(sourceCell.ErrorCellValue);
                break;

            case CellType.Formula:
                newCell.CellFormula = sourceCell.CellFormula;
                break;

            default:
                newCell.SetCellValue(string.Empty);
                break;
        }
    }
}