using Microsoft.AspNetCore.Http;

namespace HRM.Models.Payroll;

public class UploadPayrollRequest
{
    public IFormFile File { get; set; }
    public string Month { get; set; }
    public string PeriodType { get; set; }
}

public class ConfirmUploadRequest
{
    public string Month { get; set; }
    public string PeriodType { get; set; }
}