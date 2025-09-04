using System;

namespace HRM.Models.Group;

public class GroupTeamModel
{
    public int Id { get; set; }

    public int GroupTeamId { get; set; }

    public int UnitId { get; set; }

    public string UnitCode { get; set; }
    public int UnitPICEmpId { get; set; }

    public int DepartmentId { get; set; }

    public int PICEmpId { get; set; }

    public int? DepartmentPICCtrId { get; set; }
    public int DepartmentPICEmpId { get; set; }

    public string DepartmentName { get; set; }

    public string DepartmentCode { get; set; }

    public string DepartmentPIC { get; set; }
    
    public int? TeamPICCtrId { get; set; }

    public string TeamName { get; set; }

    public string TeamCode { get; set; }

    public string TeamPIC { get; set; }

    public string TeamPICCompany { get; set; }

    public int? TeamPICCompanyId { get; set; }

    public string PICDisplayName {
        get
        {
            if (TeamPICCompanyId != null && TeamPICCompanyId != CompanyId)
            {
                return $"{TeamPIC} ({TeamPICCompany})";
            }

            return TeamPIC;
        }
    }

    public string AccountableArea { get; set; }

    public string CompanyName { get; set; }

    public int NoOfMembers { get; set; }

    public int CompanyId { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string MailGroup { get; set; }

    public byte? LocalBasicAnnualLeaveBalance { get; set; }

    public byte? LocalWorkingDays { get; set; }

    public byte? LocalWorkingHours { get; set; }

    public byte? LocalOffDaysbyWeek { get; set; }

    public byte? LocalWorkingHoursbyWeek { get; set; }

    public byte? LocalYearsOfServiceForExtraLeave { get; set; }

    public byte? ExpatBasicAnnualLeaveBalance { get; set; }

    public byte? ExpatWorkingDays { get; set; }

    public byte? ExpatWorkingHours { get; set; }

    public byte? ExpatOffDaysbyWeek { get; set; }

    public byte? ExpatWorkingHoursbyWeek { get; set; }

    public byte? ExpatYearsOfServiceForExtraLeave { get; set; }

    public bool IsOutsource { get; set; }
}