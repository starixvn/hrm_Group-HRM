namespace Fanex.NexHR.Core.NPOIExtensions
{
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
                else if (value is int?)
                {
                    cell.SetCellValue((int)value);
                }
                else if (value is long?)
                {
                    cell.SetCellValue((long)value);
                }
                else if (value is double?)
                {
                    cell.SetCellValue((double)value);
                }
                else if (value is decimal?)
                {
                    cell.SetCellValue((decimal)value);
                }
                else if (value is bool?)
                {
                    cell.SetCellValue((bool)value);
                }
                else if (value is DateTime?)
                {
                    cell.SetCellValue((DateTime)value);
                }
                else if (richTextString != null)
                {
                    cell.SetCellValue(richTextString);
                }
                else
                {
                    cell.SetCellValue(string.Empty);
                }
            }
        }

        public static void SetCellValue(this ICell cell, double? value)
        {
            double result = value ?? 0;

            cell.SetCellValue(result);
        }

        public static void SetCellValue(this ICell cell, decimal value)
        {
            cell.SetCellValue(Convert.ToDouble(value));
        }

        public static void SetCellValue(this ICell cell, decimal? value)
        {
            decimal result = value ?? 0;

            cell.SetCellValue(result);
        }

        public static void SetCellValue(this ICell cell, int value)
        {
            cell.SetCellValue(Convert.ToDouble(value));
        }

        public static void SetCellValue(this ICell cell, int? value)
        {
            int result = value ?? 0;

            cell.SetCellValue(result);
        }

        public static void SetCellValue(this ICell cell, long value)
        {
            cell.SetCellValue(Convert.ToDouble(value));
        }

        public static void SetCellValue(this ICell cell, long? value)
        {
            long result = value ?? 0;

            cell.SetCellValue(result);
        }

        public static void CopyFrom(this ICell cell, ICell sourceCell)
        {
            // Copy merged cells.
            CellRangeAddress sourceRange = null;
            int i = 0;

            while (sourceRange == null && i < sourceCell.Sheet.NumMergedRegions)
            {
                CellRangeAddress range = sourceCell.Sheet.GetMergedRegion(i);

                if ((range.FirstColumn <= sourceCell.ColumnIndex)
                    && (range.LastColumn >= sourceCell.ColumnIndex)
                    && (range.FirstRow <= sourceCell.RowIndex)
                    && (range.LastRow >= sourceCell.RowIndex))
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
}