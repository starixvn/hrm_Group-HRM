namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using HRM.Models.Report;
    using HRM.Resources;
    using OfficeOpenXml;

    public interface IExportDisciplineSummaryPerYearService
    {
        byte[] Export(int fromYear, int toYear, IList<DisciplineSummaryPerYear> employees, bool enableGroupDate);
    }

    public class ExportDisciplineSummaryPerYearService : IExportDisciplineSummaryPerYearService
    {
        public byte[] Export(int fromYear, int toYear, IList<DisciplineSummaryPerYear> employees, bool enableGroupDate)
        {
            const byte LengthOfServiceColIndex = 9;
            const byte BodyRowIndex = 5;
            const byte TotalColumns = 14;
            const byte MinimumWidth = 5;
            const byte MaximumWidth = 50;
            const byte columnGroupDate = 8;

            var templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/DisciplineSummaryPerYear.xlsx");
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    ExcelWorksheet template = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("5 Years Discipline Summary");
                    int rowIndex = 1;
                    var colIndex = 1;

                    // title & header
                    template.Cells[rowIndex, colIndex, BodyRowIndex - 1, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, BodyRowIndex - 1, TotalColumns]);
                    colIndex = LengthOfServiceColIndex + 1;
                    for (var i = toYear; i >= fromYear; i--)
                    {
                        worksheet.Cells[BodyRowIndex - 1, colIndex++].Value = i;
                    }

                    // data
                    var rowNum = 1;
                    rowIndex = BodyRowIndex;
                    foreach (DisciplineSummaryPerYear employee in employees)
                    {
                        colIndex = 1;
                        template.Cells[BodyRowIndex, 1, BodyRowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Section;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Team;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Nickname;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.IsLocal ? Labels.Local : Labels.Expat;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.JoinedDate;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.LengthOfService;
                        for (var i = toYear; i >= fromYear; i--)
                        {
                            FillDisciplineCell(worksheet.Cells[rowIndex, colIndex++], employee, i);
                        }

                        rowIndex++;
                    }

                    worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
                    worksheet.View.FreezePanes(BodyRowIndex, 1);

                    if (!enableGroupDate)
                    {
                        worksheet.DeleteColumn(columnGroupDate);
                    }

                    return xlPackage.GetAsByteArray();
                }
            }
        }

        private static void FillDisciplineCell(ExcelRange cell, DisciplineSummaryPerYear employee, int year)
        {
            const byte July = 7;
            if (employee.JoinedDate.HasValue && employee.JoinedDate.Value.Date < new DateTimeOffset(year, July, 1, 0, 0, 0, TimeSpan.Zero).Date)
            {
                var discipline = employee.Disciplines.Where(x => x.Year == year).Sum(x => x.ApproverSubtractScore);
                cell.Value = discipline > 0 ? -discipline : 0;
                if (discipline > 0)
                {
                    cell.Style.Font.Color.SetColor(Color.Red);
                }
            }
            else
            {
                cell.Value = "-";
            }
        }
    }
}