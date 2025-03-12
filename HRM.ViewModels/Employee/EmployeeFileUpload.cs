namespace HRM.ViewModels.Employee;

using System;

public class EmployeeFileUpload
{
    public int EmpId { get; set; }

    public string Code { get; set; }

    public string FileName { get; set; }

    public DateTime UploadedDate { get; set; }

    public string FileLocation
    {
        get
        {
            return $"/file/uploads/profile/{Code}/{FileName}";
        }
    }
}