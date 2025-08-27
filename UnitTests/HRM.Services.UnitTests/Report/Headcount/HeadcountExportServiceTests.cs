using System.Collections.Generic;
using HRM.Services.Implement.Report.Headcount;
using HRM.Models.Report.HeadcountReport;
using Xunit;
using System.IO;
using NSubstitute;
using GO.UserControl.Client.Core.Session;
using HRM.Core;

namespace HRM.Services.UnitTests.Report.Headcount;

public class HeadcountExportServiceTests
{
    public HeadcountExportServiceTests()
    {
        var _userControlSession = Substitute.For<IUserControlSession>();
        _userControlSession.Get<float>(Arg.Any<string>()).Returns(7);
        
        DateTimeExtensions.Configure(_userControlSession);
    }

    [Fact]
    public void Export_WithValidData_ShouldReturnExcelBytes()
    {
        // Arrange
        var service = new HeadcountExportService();
        var templatePath = CreateTestTemplate();
        var headcountData = CreateTestHeadcountData();
        var selectedYear = 2024;

        // Act
        var result = service.Export(templatePath, headcountData, selectedYear);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
        Assert.True(result.Length > 1000);
    }

    private static string CreateTestTemplate()
    {
        var tempPath = Path.GetTempFileName();
        var templatePath = Path.ChangeExtension(tempPath, ".xlsx");
        
        if (File.Exists(tempPath))
        {
            File.Delete(tempPath);
        }

        using var package = new OfficeOpenXml.ExcelPackage();
        for (int i = 0; i < 6; i++)
        {
            var worksheet = package.Workbook.Worksheets.Add($"Sheet{i + 1}");
            worksheet.Cells[1, 1].Value = "Test Header";
        }
        
        package.SaveAs(new FileInfo(templatePath));
        return templatePath;
    }

    private static HeadcountReportModel CreateTestHeadcountData()
    {
        return new HeadcountReportModel
        {
            ReportByUnit =
            [
                new UnitReport
                {
                    Label1 = "Test Unit",
                    IsUnitCategory = true,
                    Details = new List<ReportModel>
                    {
                        new()
                        {
                            Month = 1,
                            NewHire = 5,
                            ProbationResign = 1,
                            ConfirmedResign = 2,
                            CurrentHeadCount = 100
                        }
                    }
                }
            ],
            ReportByCompany =
            [
                new UnitReport
                {
                    Label1 = "Test Company",
                    Label2 = "Test Department",
                    IsUnitCategory = false,
                    Details = new List<ReportModel>
                    {
                        new()
                        {
                            Month = 1,
                            NewHire = 10,
                            ProbationResign = 2,
                            ConfirmedResign = 3,
                            CurrentHeadCount = 200
                        }
                    }
                }
            ],
            ReportByDetails =
            [
                new UnitReport
                {
                    Label1 = "Test Unit",
                    Label2 = "Test Department",
                    Label3 = "Test Position",
                    IsUnitCategory = false,
                    Details = new List<ReportModel>
                    {
                        new()
                        {
                            Month = 1,
                            NewHire = 3,
                            ProbationResign = 0,
                            ConfirmedResign = 1,
                            CurrentHeadCount = 50
                        }
                    }
                }
            ],
            ReportByUnitCountry =
            [
                new UnitReport
                {
                    Label1 = "Test Country",
                    Label3 = "Test Position",
                    Details = new List<ReportModel>
                    {
                        new()
                        {
                            Month = 1,
                            CurrentHeadCount = 75
                        }
                    }
                }
            ],
            ReportByUnitCountryLocal =
            [
                new UnitReport
                {
                    Label1 = "Test Local",
                    Label3 = "Test Position",
                    Details = new List<ReportModel>
                    {
                        new()
                        {
                            Month = 1,
                            CurrentHeadCount = 60
                        }
                    }
                }
            ],
            ReportByUnitCountryExpat =
            [
                new UnitReport
                {
                    Label1 = "Test Expat",
                    Label3 = "Test Position",
                    Details = new List<ReportModel>
                    {
                        new()
                        {
                            Month = 1,
                            CurrentHeadCount = 15
                        }
                    }
                }
            ]
        };
    }
}
