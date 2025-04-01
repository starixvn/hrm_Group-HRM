namespace HRM.Core.NPOIExtensions;

using NPOI.SS.UserModel;

public static class HSSFRowExtensions
{
    public static void WriteCells(this IRow row, int colIndex, object[] values)
    {
        var id = colIndex;

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