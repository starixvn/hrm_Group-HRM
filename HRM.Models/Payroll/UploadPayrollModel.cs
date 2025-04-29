using System;
using Microsoft.AspNetCore.Http;

namespace HRM.Models.Payroll;

public class UploadPayrollModel
{
    public IFormFile File { get; set; }
    public string SavedFileName { get; set; }
    public int AuthorId { get; set; }
    public int CompanyId { get; set; }
    public DateTime Month { get; set; }
    public int PeriodType { get; set; }
}
