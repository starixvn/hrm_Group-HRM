namespace HRM.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.Enums;
using Resources;
using GO.UserControl.Client.Core.Models.Role;
using HRM.Models.Log;

public class Employee : EntityBase<int>
{
    public int? CtrId { get; set; }

    public int UserId { get; set; }
    public string DomainUsername { get; set; }

    [Display(ResourceType = typeof(Labels), Name = nameof(Labels.FullName))]
    [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage))]
    public string FullName { get; set; }

    public string NickName { get; set; }

    public string EmployeeCard { get; set; }

    public string ChineseName { get; set; }

    public string Password { get; set; }

    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public string PlaceOfBirth { get; set; }

    public string Ethnic { get; set; }

    public string Native { get; set; }

    public string Religion { get; set; }

    public DateTime? DateOfIdCard { get; set; }

    public string PlaceOfIdCard { get; set; }

    public string Qualification { get; set; }

    public string Major { get; set; }

    public string HomePhone { get; set; }

    [EmailAddress(ErrorMessage = "{0} is invalid.")]
    public string Email { get; set; }

    public int? JobTitleId { get; set; }

    public int? JobLevelId { get; set; }

    public short? ManagementLevelId { get; set; }
    public int? SalaryStructureId { get; set; }
    public string Salary { get; set; }

    public bool Enabled { get; set; }

    public bool IsDeleted { get; set; }

    public string Phone { get; set; }

    public byte? Gender { get; set; }

    public string UnitCode { get; set; }

    public string UnitName { get; set; }

    public Team Team { get; set; } = new();

    [DataType(DataType.Date)]
    public DateTime? TeamDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? JoinedDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? ConfirmedDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? ResignedDate { get; set; }

    public string ResignedReason { get; set; }

    public string LetsTalk { get; set; }

    public byte Marriage { get; set; }

    public string MarriageName => ((MarriageStatus)Marriage).ToString();

    [DataType(DataType.Date)]
    public DateTime? MilitaryFromDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? MilitaryToDate { get; set; }

    public string Remark { get; set; }

    public DateTime? LastLogin { get; set; }

    public decimal AnnualLeaveBalance { get; set; }

    public string JobTitle { get; set; }

    public string JobLevel { get; set; }

    public string ManagementLevel { get; set; }

    [DataType(DataType.Date)]
    public DateTime? PromotionDate { get; set; }

    [Display(Name = "Permanent Address")]
    public string Address { get; set; }

    public int? SubCompanyId { get; set; }

    public string SubCompanyName { get; set; }

    public string CompanyName { get; set; }

    public string CurrentAddress { get; set; }

    public string CardNo { get; set; }

    public string PassportNo { get; set; }

    public DateTime? PassportExp { get; set; }

    public string Verifier { get; set; }

    public string Approver { get; set; }

    public string Disciplinarian { get; set; }

    [Display(Name = "Emp. Shift List")]
    public string EmpShiftList { get; set; }

    [Display(Name = "Acting Emp. List")]
    public string ActingEmpList { get; set; }

    public bool IsLocal { get; set; }

    [DataType(DataType.Date)]
    public DateTime? CompanyDate { get; set; }

    [Range(1, 31, ErrorMessage = "Working Days must be between 1 and 31.")]
    public byte WorkingDays { get; set; }

    [Range(1, 24, ErrorMessage = "Working Hours must be between 1 and 24.")]
    public byte WorkingHours { get; set; }

    [Range(1, 120, ErrorMessage = "Working Hours per week must be between 1 and 120.")]
    public byte? WorkingHoursbyWeek { get; set; }

    public decimal KeepInViewBalance { get; set; }

    public bool CanOT { get; set; }

    [Range(0, 24, ErrorMessage = "Special OT must be between 0 and 24.")]
    public decimal? SpecialOT { get; set; }

    [Display(Name = "Special User")]
    public bool IsTopManager { get; set; }

    [Display(Name = "Default Calendar")]
    public bool IsDefaultSchedule { get; set; }

    public IList<Country> Coutries { get; set; }

    public string CountryCode { get; set; }

    public string CountryName { get; set; }

    public int CompanyId { get; set; }

    public bool IsShowLeftNavigation { get; set; }

    [Range(0, 5, ErrorMessage = "Number of Off days must be between 0 and 5.")]
    public decimal OffDaysbyWeek { get; set; }

    [Required]
    public override string Code { get; set; }

    public bool IsExpatUseHoliday { get; set; }

    public bool PasswordReset { get; set; }

    public IList<Role> Roles { get; set; }

    public string[] Roles2Array
    {
        get => GetRoles2Array();

        set
        {
            Roles = value != null ? value.Select(i => new Role { RoleId = Convert.ToInt16(i) }).ToList() : [];
        }
    }

    public string Roles2String
    {
        get
        {
            return Roles == null ? string.Empty : string.Join(", ", Roles.Select(i => i.Name));
        }
    }

    #region Contract

    public string BankAccount { get; set; }

    public string TaxCode { get; set; }

    public string SocialInsuranceNo { get; set; }

    public string RegisteredHospital { get; set; }

    public string ExtensionNo { get; set; }

    #endregion Contract

    public string DisplayName => string.IsNullOrWhiteSpace(NickName) ? FullName : NickName + " (" + FullName + ")";

    public string DisplayNameWithCompany => NickName + " (" + CompanyName + ")";

    [Display(Name = "Pickup Remark")]
    public string PickupRemark { get; set; }

    [Display(Name = "Auto-Approved")]
    public bool AutoApproveRequest { get; set; }

    public IEnumerable<LogObjectDetail> HistoryWork { get; set; }

    private string[] GetRoles2Array() => Roles?.Select(i => i.RoleId.ToString()).ToArray();
}