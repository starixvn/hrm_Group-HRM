namespace HRM.Core;

using System.IO;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOIExtensions;

public class ExcelTemplateWriter(HSSFWorkbook template, string sheetName)
{
    #region Attributes

    private readonly ISheet _templateSheet = template.GetSheetAt(0);
    private readonly ISheet _sheet = template.CreateSheet(sheetName);

    #endregion Attributes

    #region Constructors

    public ExcelTemplateWriter(string filePath, string sheetName = "Result")
        : this(new FileStream(filePath, FileMode.Open, FileAccess.Read), sheetName)
    {
    }

    public ExcelTemplateWriter(FileStream fs, string sheetName = "Result")
        : this(new HSSFWorkbook(fs), sheetName)
    {
	    fs?.Close();
    }

    #endregion Constructors

    #region Properties

    public int RowId { get; private set; }

    #endregion Properties

    public byte[] WriteContentToBytes()
    {
        using var output = new MemoryStream();
        _sheet.Workbook.RemoveSheetAt(0);
        _sheet.Workbook.Write(output);

        return output.ToArray();
    }

    public void WriteCellRange(int rowIndex, int totalRow, int colIndex, int totalCol, object value)
    {
        var cellRange = new CellRangeAddress(rowIndex, rowIndex + totalRow - 1, colIndex, colIndex + totalCol - 1);
        _sheet.AddMergedRegion(cellRange);

        var cell = _sheet.GetCell(rowIndex, colIndex);
        cell.SetCellValue(value);
    }

    public void CopyCellStyle(int sourceRow, int sourceCol, int desRow, int desCol)
    {
        var cellStyle = _templateSheet.GetCell(sourceRow, sourceCol).CellStyle;
        var cell = _sheet.CreateCell(desRow, desCol);
        cell.CellStyle = cellStyle;
    }

    public void WriteCellValue(int rowIndex, int ColIndex, object value)
    {
        var cell = _sheet.CreateCell(rowIndex, ColIndex);
        cell.SetCellValue(value);
    }

    #region Auto resize columns' width

    public void AutoSizeColumns(int from, int to)
    {
        for (var i = from; i <= to; i++)
        {
            _sheet.AutoSizeColumn(i, false);
        }
    }

    public void AutoSizeColumns(int total)
    {
        AutoSizeColumns(0, total - 1);
    }

    #endregion Auto resize columns' width

    #region Synchronize columns' width

    public void SyncColumnsWidth(int from, int to)
    {
        for (var i = from; i <= to; i++)
        {
            _sheet.SetColumnWidth(i, _templateSheet.GetColumnWidth(i));
        }
    }

    public void SyncColumnsWidth(int total)
    {
        SyncColumnsWidth(0, total - 1);
    }

    #endregion Synchronize columns' width

    #region Row related methods

    public void WriteRow(int rowIndex, object[] values)
    {
        var row = CreateRow(rowIndex);
        row.WriteCells(0, values);
    }

    public void WriteRow(int rowIndex, object value)
    {
        if (value != null)
        {
            WriteRow(rowIndex, value.GetType().GetProperties().Select(propertyInfo => propertyInfo.GetValue(value, null)).ToArray());
        }
    }

    public IRow CopyRows(int rowIndex, int total)
    {
        return CreateRows(rowIndex, total);
    }

    public void RemoveColumn(int colIndex)
    {
        _sheet.RemoveColumn(colIndex);
    }

    private IRow CreateRow(int rowId)
    {
        return CreateRows(rowId, 1);
    }

    private IRow CreateRows(int rowIndex, int total)
    {
        _sheet.CopyRows(_templateSheet, rowIndex, RowId, total);
        RowId += total;
        return _sheet.GetRow(RowId - total);
    }

    #endregion Row related methods

    public void CreateFreezePane(int colSplit, int rowSplit)
    {
        _sheet.CreateFreezePane(colSplit, rowSplit);
    }
}