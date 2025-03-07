using HRM.Services.Implement.Request;

namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using HRM.Core;
    using HRM.Core.Enums;
    using HRM.ViewModels;
    using Microsoft.AspNetCore.Hosting;
    using Models;
    using Models.Request;
    using OfficeOpenXml;

    public interface IExportExcelAttendanceReportService
    {
        byte[] Export(IEnumerable<EmployeeAttendanceModel> attendances, DateTime fromDate, DateTime toDate);
    }

#pragma warning disable S109 // Magic numbers should not be used

    public class ExportExcelAttendanceReportService(IWebHostEnvironment hostingEnvironment) : IExportExcelAttendanceReportService
    {
        private const byte RowStartBody = 3;
        private const byte RowHighlight = 4;
        private const byte RowOffdate = 5;
        private const byte RowWeekend = 6;
        private const byte TotalColumns = 17;
        private const byte MinimumWidth = 5;
        private const byte MaximumWidth = 50;

        public byte[] Export(IEnumerable<EmployeeAttendanceModel> attendances, DateTime fromDate, DateTime toDate)
        {
            var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, @"ExcelTemplates\AttendanceReport.xlsx");

            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    var template = templatePackage.Workbook.Worksheets[0];
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Attendance Report");
                    var rowIndex = 1;
                    var colIndex = 1;

                    // title
                    template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    rowIndex++;

                    // header
                    template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    rowIndex++;

                    // data
                    var rowNum = 1;
                    foreach (var attendance in attendances)
                    {
                        colIndex = 1;
                        var templateRowIndex = GetTemplateRowIndex(attendance);
                        template.Cells[templateRowIndex, 1, templateRowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        var requests = RequestsRemark(attendance);
                        var shiftCode = !string.IsNullOrEmpty(attendance.ShiftCode) ? attendance.ShiftCode : string.Empty;
                        var totalWorkHours = attendance.TotalWorkHours != 0 ? Utilities.TotalHoursToTime((float)attendance.TotalWorkHours) : string.Empty;
                        var overtime = attendance.IsOvertime ? attendance.FormattedOvertimeStatus + " (" + attendance.OvertimeHours.ToString() + ")" : string.Empty;

                        worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
                        worksheet.Cells[rowIndex, colIndex++].Value = attendance.AttendanceDate;
                        worksheet.Cells[rowIndex, colIndex++].Value = attendance.DepartmentCode;
                        worksheet.Cells[rowIndex, colIndex++].Value = attendance.SectionCode;
                        worksheet.Cells[rowIndex, colIndex++].Value = attendance.TeamCode;
                        worksheet.Cells[rowIndex, colIndex++].Value = attendance.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = attendance.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = attendance.NickName;
                        worksheet.Cells[rowIndex, colIndex++].Value = shiftCode;
                        worksheet.Cells[rowIndex, colIndex++].Value = attendance.InFixed.HasValue ? attendance.InFixed.Value.ToClientTimeZone() : (object)"";
                        worksheet.Cells[rowIndex, colIndex++].Value = attendance.OutFixed.HasValue ? attendance.OutFixed.Value.ToClientTimeZone() : (object)"";
                        worksheet.Cells[rowIndex, colIndex++].Value = totalWorkHours;
                        worksheet.Cells[rowIndex, colIndex++].Value = Utilities.MinuteToTime(attendance.LateIn);
                        worksheet.Cells[rowIndex, colIndex++].Value = attendance.IsOffDay == 1 && !string.IsNullOrWhiteSpace(shiftCode) ?
                                            string.Empty : Utilities.MinuteToTime(attendance.EarlyOut);
                        worksheet.Cells[rowIndex, colIndex++].Value = requests;
                        worksheet.Cells[rowIndex, colIndex++].Value = overtime;
                        worksheet.Cells[rowIndex, colIndex].Value = attendance.Notes;

                        rowIndex++;
                        rowNum++;
                    }

                    worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
                    worksheet.View.FreezePanes(RowStartBody, 1);
                    return xlPackage.GetAsByteArray();
                }
            }
        }

        private static byte GetTemplateRowIndex(EmployeeAttendanceModel attendance)
        {
            byte rowIndex;
            if (attendance.IsOffdayOT)
            {
                rowIndex = RowHighlight;
            }
            else if (attendance.IsPublicHoliday && !string.IsNullOrEmpty(attendance.ShiftCode))
            {
                rowIndex = RowOffdate;
            }
            else if (attendance.IsWeekend)
            {
                rowIndex = RowWeekend;
            }
            else
            {
                rowIndex = RowStartBody;
            }

            return rowIndex;
        }

        private static string RequestsRemark(EmployeeAttendanceModel attendance)
        {
            var requests = string.Empty;
            var requestsBuilder = new StringBuilder();
            if (attendance.RequestList != null && attendance.RequestList.Any())
            {
                foreach (var item in attendance.RequestList)
                {
                    var actualLateInErlyOut = item.RequestType.Id == (byte)RequestType.LateIn ? attendance.LateIn : attendance.EarlyOut;
                    var request = GetRequestEntity(item, actualLateInErlyOut);
                    requestsBuilder.Append("- ");
                    requestsBuilder.Append(request.RequestType.Name);
                    requestsBuilder.Append(": ");
                    requestsBuilder.Append(RequestService.ToRequestMessage(request, isNoFormat: true));
                    requestsBuilder.Append("\r\n- Create date: ");
                    requestsBuilder.Append(request.CreatedDate.ToDateMonthYearFormatedDate());
                    requestsBuilder.Append((string.IsNullOrEmpty(request.Remark) ? string.Empty : "\r\n" + "- Remark: " + request.Remark));
                    requestsBuilder.Append("\r\n");
                }

                requests = requestsBuilder.ToString(0, requestsBuilder.Length - 2);
            }

            return requests;
        }

        private static RequestEntity GetRequestEntity(EmployeeAttendanceRequest request, int actualLateInErlyOut)
        {
            var currentDateTime = DateTimeOffset.Now.DateTime;
            var requestEntity = new RequestEntity
            {
                RequestType = request.RequestType,
                Remark = request.Remark,
                RequestedDate = request.RequestDate,
                FromDate = request.FromDate ?? currentDateTime,
                ToDate = request.ToDate ?? currentDateTime,
                CreatedDate = request.CreatedDate,
                TotalDays = request.TotalDays ?? 0,
                LeaveType = request.LeaveType,
                FromTime = request.FromTime ?? currentDateTime,
                ToTime = request.ToTime ?? currentDateTime,
                FromShift = new Shift
                {
                    Name = request.FromShift
                },
                ToShift = new Shift
                {
                    Name = request.ToShift
                },
                TotalMins = request.TotalMins ?? 0,
                ActualLateInEarlyOut = actualLateInErlyOut
            };

            return requestEntity;
        }
    }
}