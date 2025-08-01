namespace HRM.Models;

using Core;
using HRM.Core.Models;
using System;
using System.Collections.Generic;

public class EmployeeWorkFromHome
{
    private DateTime serverModifiedDate;
    public int EmployeeWorkFromHomeId { get; set; }

    public int EmpId { get; set; }
    public string Code { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }

    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public string DepartmentName { get; set; }
    public string TeamName { get; set; }

    public int ModifiedBy { get; set; }

    public string Modifier { get; set; }

    public DateTime ModifiedDate
    {
        get => serverModifiedDate;
        set => serverModifiedDate = value.ToClientTimeZone();
    }

    public string Remark { get; set; }
    public IEnumerable<S3FileObject> AttachFiles { get; set; }

    public string Location { get; set; }
}