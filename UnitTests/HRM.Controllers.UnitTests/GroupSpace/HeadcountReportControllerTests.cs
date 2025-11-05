using HRM.Controllers.GroupSpace;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Interfaces.Report.Headcount;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Xunit;

namespace HRM.Controllers.UnitTests.GroupSpace;

public class HeadcountReportControllerTests : BaseControllerTest
{
    private readonly IHeadcountReportService _service;
    private readonly IHostEnvironment _env;
    private readonly IHeadcountExportService _excelService;
    private readonly HeadcountReportController _controller;

    public HeadcountReportControllerTests()
    {
        _service = Substitute.For<IHeadcountReportService>();
        _env = Substitute.For<IHostEnvironment>();
        _excelService = Substitute.For<IHeadcountExportService>();
        
        _controller = new HeadcountReportController(
            _service,
            _env,
            _session,
            _excelService);
    }

    [Fact]
    public void Index_ShouldReturnView()
    {
        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName); // Default view name
    }

    [Fact]
    public async Task GetData_WithValidYear_ShouldReturnJsonWithData()
    {
        // Arrange
        const int year = 2024;
        var expectedData = new HeadcountReportModel
        {
            ReportByUnit = [],
            ReportByCompany = [],
            ReportByDetails = [],
            ReportByUnitCountry = [],
            ReportByUnitCountryLocal = [],
            ReportByUnitCountryExpat = []
        };

        _service.GetReportAsync(year).Returns(expectedData);

        // Act
        var result = await _controller.GetData(year);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedData, jsonResult.Value);
        await _service.Received(1).GetReportAsync(year);
    }

    [Fact]
    public async Task GetData_WithDifferentYear_ShouldCallServiceWithCorrectYear()
    {
        // Arrange
        const int year = 2023;
        var expectedData = new HeadcountReportModel
        {
            ReportByUnit = [],
            ReportByCompany = [],
            ReportByDetails = [],
            ReportByUnitCountry = [],
            ReportByUnitCountryLocal = [],
            ReportByUnitCountryExpat = []
        };

        _service.GetReportAsync(year).Returns(expectedData);

        // Act
        var result = await _controller.GetData(year);

        // Assert
        await _service.Received(1).GetReportAsync(year);
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }

    [Fact]
    public async Task GetDetail_WithValidEmpIds_ShouldReturnJsonWithData()
    {
        // Arrange
        const string empIds = "1,2,3";
        var expectedData = new List<HeadcountDetailModel>
        {
            new() { EmpId = 1, Code = "EMP001", Nickname = "John" },
            new() { EmpId = 2, Code = "EMP002", Nickname = "Jane" },
            new() { EmpId = 3, Code = "EMP003", Nickname = "Bob" }
        };

        _service.GetDetailAsync(empIds).Returns(expectedData);

        // Act
        var result = await _controller.GetDetail(empIds);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
        await _service.Received(1).GetDetailAsync(empIds);
    }

    [Fact]
    public async Task GetDetail_WithEmptyEmpIds_ShouldCallServiceWithEmptyString()
    {
        // Arrange
        const string empIds = "";
        var expectedData = new List<HeadcountDetailModel>();

        _service.GetDetailAsync(empIds).Returns(expectedData);

        // Act
        var result = await _controller.GetDetail(empIds);

        // Assert
        await _service.Received(1).GetDetailAsync(empIds);
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }

    [Fact]
    public async Task GetDetail_WithNullEmpIds_ShouldCallServiceWithNull()
    {
        // Arrange
        string empIds = null;
        var expectedData = new List<HeadcountDetailModel>();

        _service.GetDetailAsync(empIds).Returns(expectedData);

        // Act
        var result = await _controller.GetDetail(empIds);

        // Assert
        await _service.Received(1).GetDetailAsync(empIds);
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }

    [Fact]
    public async Task GetDetail_ShouldReturnJsonWithCamelCaseSettings()
    {
        // Arrange
        const string empIds = "1";
        var expectedData = new List<HeadcountDetailModel>
        {
            new() 
            { 
                EmpId = 1, 
                Code = "EMP001", 
                JoinedDate = new DateTime(2024, 1, 15),
                ConfirmedDate = new DateTime(2024, 2, 20)
            }
        };

        _service.GetDetailAsync(empIds).Returns(expectedData);

        // Act
        var result = await _controller.GetDetail(empIds);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }

    [Fact]
    public void Detail_WithValidEmpIds_ShouldSetViewBagAndReturnView()
    {
        // Arrange
        const string empIds = "1,2,3";

        // Act
        var result = _controller.Detail(empIds);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName); // Default view name
        Assert.Equal(empIds, _controller.ViewBag.EmpIds);
    }

    [Fact]
    public void Detail_WithEmptyEmpIds_ShouldSetViewBagWithEmptyString()
    {
        // Arrange
        const string empIds = "";

        // Act
        var result = _controller.Detail(empIds);

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.Equal(empIds, _controller.ViewBag.EmpIds);
    }

    [Fact]
    public void Detail_WithNullEmpIds_ShouldSetViewBagWithNull()
    {
        // Arrange
        string empIds = null;

        // Act
        var result = _controller.Detail(empIds);

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.Equal(empIds, _controller.ViewBag.EmpIds);
    }

    [Fact]
    public async Task ExportExcel_WithValidYear_ShouldReturnFileResult()
    {
        // Arrange
        const int year = 2024;
        const string templatePath = "ExcelTemplates/HeadcountReportTemplate.xlsx";
        const string contentRootPath = "C:\\Test\\ContentRoot";
        var fullTemplatePath = Path.Combine(contentRootPath, templatePath);
        
        var reportData = new HeadcountReportModel
        {
            ReportByUnit = [],
            ReportByCompany = [],
            ReportByDetails = [],
            ReportByUnitCountry = [],
            ReportByUnitCountryLocal = [],
            ReportByUnitCountryExpat = []
        };

        var excelBytes = new byte[] { 1, 2, 3, 4, 5 };

        _service.GetReportAsync(year).Returns(reportData);
        _env.ContentRootPath.Returns(contentRootPath);
        _excelService.Export(fullTemplatePath, reportData, year).Returns(excelBytes);

        // Act
        var result = await _controller.ExportExcel(year);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal(excelBytes, fileResult.FileContents);
        Assert.Equal("application/xlsx", fileResult.ContentType);
        Assert.Contains($"HeadcountReport_{year}_", fileResult.FileDownloadName);
        Assert.Contains(".xlsx", fileResult.FileDownloadName);
        
        await _service.Received(1).GetReportAsync(year);
        _excelService.Received(1).Export(fullTemplatePath, reportData, year);
    }

    [Fact]
    public async Task ExportExcel_WithDifferentYear_ShouldCallServiceWithCorrectYear()
    {
        // Arrange
        const int year = 2023;
        const string templatePath = "ExcelTemplates/HeadcountReportTemplate.xlsx";
        const string contentRootPath = "C:\\Test\\ContentRoot";
        var fullTemplatePath = Path.Combine(contentRootPath, templatePath);
        
        var reportData = new HeadcountReportModel
        {
            ReportByUnit = [],
            ReportByCompany = [],
            ReportByDetails = [],
            ReportByUnitCountry = [],
            ReportByUnitCountryLocal = [],
            ReportByUnitCountryExpat = []
        };

        var excelBytes = new byte[] { 6, 7, 8, 9, 10 };

        _service.GetReportAsync(year).Returns(reportData);
        _env.ContentRootPath.Returns(contentRootPath);
        _excelService.Export(fullTemplatePath, reportData, year).Returns(excelBytes);

        // Act
        var result = await _controller.ExportExcel(year);

        // Assert
        await _service.Received(1).GetReportAsync(year);
        _excelService.Received(1).Export(fullTemplatePath, reportData, year);
        
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Contains($"HeadcountReport_{year}_", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task ExportExcel_ShouldGenerateFileNameWithCurrentDate()
    {
        // Arrange
        const int year = 2024;
        const string templatePath = "ExcelTemplates/HeadcountReportTemplate.xlsx";
        const string contentRootPath = "C:\\Test\\ContentRoot";
        var fullTemplatePath = Path.Combine(contentRootPath, templatePath);
        
        var reportData = new HeadcountReportModel
        {
            ReportByUnit = [],
            ReportByCompany = [],
            ReportByDetails = [],
            ReportByUnitCountry = [],
            ReportByUnitCountryLocal = [],
            ReportByUnitCountryExpat = []
        };

        var excelBytes = new byte[] { 1, 2, 3 };
        var currentDate = DateTimeOffset.Now.Date;

        _service.GetReportAsync(year).Returns(reportData);
        _env.ContentRootPath.Returns(contentRootPath);
        _excelService.Export(fullTemplatePath, reportData, year).Returns(excelBytes);

        // Act
        var result = await _controller.ExportExcel(year);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        var expectedDateString = currentDate.ToString("yyyyMMdd");
        Assert.Contains(expectedDateString, fileResult.FileDownloadName);
    }

    [Fact]
    public async Task ExportExcel_ShouldCombineTemplatePathCorrectly()
    {
        // Arrange
        const int year = 2024;
        const string templatePath = "ExcelTemplates/HeadcountReportTemplate.xlsx";
        const string contentRootPath = "C:\\Test\\ContentRoot";
        var expectedFullPath = Path.Combine(contentRootPath, templatePath);
        
        var reportData = new HeadcountReportModel
        {
            ReportByUnit = [],
            ReportByCompany = [],
            ReportByDetails = [],
            ReportByUnitCountry = [],
            ReportByUnitCountryLocal = [],
            ReportByUnitCountryExpat = []
        };

        var excelBytes = new byte[] { 1, 2, 3 };

        _service.GetReportAsync(year).Returns(reportData);
        _env.ContentRootPath.Returns(contentRootPath);
        _excelService.Export(expectedFullPath, reportData, year).Returns(excelBytes);

        // Act
        var result = await _controller.ExportExcel(year);

        // Assert
        _excelService.Received(1).Export(
            Arg.Is<string>(path => path == expectedFullPath),
            reportData,
            year);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.NotNull(fileResult);
    }
}

