using System;
using Microsoft.AspNetCore.Http;

namespace HRM.Models.Payroll;

public class UploadPayrollModel
{
    public IFormFile File { get; set; }
    public string SavedFileName { get; set; }
    public int ActorId { get; set; }
    public int CompanyId { get; set; }
    public DateTime Month { get; set; }
    public short PeriodType { get; set; }
}
