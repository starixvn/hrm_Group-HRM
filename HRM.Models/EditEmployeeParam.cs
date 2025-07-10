namespace HRM.Models;

using System;

public class EditEmployeeParam
{
    public int EmployeeId { get; set; }
    public string Code { get; set; }
    public string DomainUsername { get; set; }
    public string FullName { get; set; }
    public string Nickname { get; set; }
    public string EmployeeCard { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Email { get; set; }
    public int? JobTitleId { get; set; }
    public int? JobLevelId { get; set; }
    public short? ManagementLevelId { get; set; }
    public DateTime? PromotionDate { get; set; }
    public bool Enabled { get; set; }
    public string Phone { get; set; }
    public byte? Gender { get; set; }
    public int TeamId { get; set; }
    public DateTime? TeamDate { get; set; }
    public string Roles { get; set; }
    public DateTime? JoinedDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public DateTime? ResignedDate { get; set; }
    public string ResignedReason { get; set; }
    public DateTime? MilitaryFromDate { get; set; }
    public DateTime? MilitaryToDate { get; set; }
    public string Skype { get; set; }
    public byte Marriage { get; set; }
    public string PermanentAddress { get; set; }
    public string CurrentAddress { get; set; }
    public string IdCardNumber { get; set; }
    public string PassportNo { get; set; }
    public DateTime? PassportExp { get; set; }
    public string ChineseName { get; set; }
    public bool CanOT { get; set; }
    public decimal? SpecialOT { get; set; }
    public bool IsLocal { get; set; }
    public byte WorkingDays { get; set; }
    public byte WorkingHours { get; set; }
    public byte? WorkingHoursbyWeek { get; set; }
    public DateTime? CompanyDate { get; set; }
    public string CountryCode { get; set; }
    public bool IsTopManager { get; set; }
    public decimal? OffDaysbyWeek { get; set; }
    public bool IsExpatUseHoliday { get; set; }
    public string PlaceOfBirth { get; set; }
    public string Ethnic { get; set; }
    public string Native { get; set; }
    public string Religion { get; set; }
    public DateTime? DateOfIdCard { get; set; }
    public string PlaceOfIdCard { get; set; }
    public string HomePhone { get; set; }
    public string Qualification { get; set; }
    public string Major { get; set; }
    public string PickupRemark { get; set; }
    public string LetsTalk { get; set; }
    public int? SubCompanyId { get; set; }
    public int? SalaryStructureId { get; set; }
    public string Salary { get; set; }
}