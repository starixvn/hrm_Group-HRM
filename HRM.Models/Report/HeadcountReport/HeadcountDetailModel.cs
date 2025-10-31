using System;

namespace HRM.Models.Report.HeadcountReport;

public class HeadcountDetailModel
{
    public int? CtrId { get; set; }

    public string CtrIdFormatted => CtrId is not null ? CtrId.ToString().PadLeft(6, '0') : string.Empty;
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }
    public int Gender { get; set; }
    public DateTime? DOB { get; set; }
    public bool Enabled { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? JoinedDate { get; set; }
    public DateTime? ResignedDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public bool IsLocal { get; set; }
    public bool IsSpecialUser { get; set; }
    public DateTime? CompanyDate { get; set; }
    public string Company { get; set; }
    public string SubCompany { get; set; }
    public string BU { get; set; }
    public string TeamName { get; set; }
    public string DepartmentName { get; set; }
    public string Location { get; set; }
}

