namespace HRM.Core.NPOIExtensions;

using NPOI.SS.UserModel;

public static class HSSFRowExtensions
{
    public static ICell CreateCell(this IRow row, int colIndex, ICellStyle cellStyle)
    {
        ICell cell = null;

        if (row != null)
        {
            cell = row.CreateCell(colIndex);
            cell.CellStyle = cellStyle;
        }

        return cell;
    }

    public static void WriteCells(this IRow row, int colIndex, object[] values)
    {
        int id = colIndex;

        if (row != null && values != null)
        {
            foreach (var value in values)
            {
                row.GetCell(id)?.SetCellValue(value);
                id++;
            }
        }
    }
}