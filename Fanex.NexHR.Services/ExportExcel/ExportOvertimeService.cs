namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Models.Overtime;
    using OfficeOpenXml;

    public interface IExportOvertimeService
    {
        byte[] Export(IEnumerable<MonthlyOvertimeRequest> memberRequests, bool isMemberOvertime);
    }

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportOvertimeService : IExportOvertimeService
    {
        public byte[] Export(IEnumerable<MonthlyOvertimeRequest> memberRequests, bool isMemberOvertime)
        {
            const byte TotalColumns = 14;
            const byte StartBodyRow = 3;
            const byte StartTotal = 8;
            const byte LineOfTotal = 4;

            string sheetName = "Employee Overtime Report";
            string templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/EmployeeOverTimeTemplate.xlsx");

            if (isMemberOvertime)
            {
                sheetName = "Member Overtime Report";
            }

            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet workSheet = xlPackage.Workbook.Worksheets.Add(sheetName);

                    int rowIndex = 1;
                    int colIndex = 1;
                    int count = 1;

                    // title
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(workSheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    workSheet.Cells[rowIndex, colIndex].Value = string.Format(sheetName);
                    rowIndex++;

                    // header
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex + 1, TotalColumns].Copy(workSheet.Cells[rowIndex, colIndex, rowIndex + 1, TotalColumns]);
                    rowIndex++;

                    foreach (var detail in memberRequests)
                    {
                        templateWorksheet.Cells[StartBodyRow, 1, StartBodyRow, TotalColumns].Copy(workSheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        workSheet.Cells[rowIndex, colIndex++].Value = count;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.Requester.Department;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.Requester.Section;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.Team;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.FullName;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.Nickname;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.Month.ToString("MMM-yyyy");
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.NormalHours;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.OffHours;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.PublicHours;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.SpecialHours;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.Verifier.Nickname;
                        workSheet.Cells[rowIndex, colIndex++].Value = detail.Approver.Nickname;
                        workSheet.Cells[rowIndex, colIndex].Value = detail.Status;

                        rowIndex++;
                        colIndex = 1;
                        count++;
                    }

                    // summary
                    if (memberRequests.Any())
                    {
                        templateWorksheet.Cells[LineOfTotal, 1, LineOfTotal, TotalColumns].Copy(workSheet.Cells[rowIndex, 1, rowIndex, TotalColumns]);
                        const byte Total = 3;
                        for (var i = 0; i <= Total; i++)
                        {
                            colIndex = StartTotal + i;
                            var excelAddress = $"{workSheet.Cells[StartBodyRow, colIndex].Address}:{workSheet.Cells[rowIndex - 1, colIndex].Address}";
                            workSheet.Cells[rowIndex, colIndex].Formula = string.Format(Constants.SummaryFormula, excelAddress);
                        }
                    }

                    workSheet.Cells.AutoFitColumns();
                    return xlPackage.GetAsByteArray();
                }
            }
        }
    }
}