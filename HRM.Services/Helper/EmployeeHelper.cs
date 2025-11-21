using Fanex.Data;
using HRM.Models;
namespace HRM.Services.Helper;

public static class EmployeeHelper
{
    public static EditEmployeeParam MappingEditEmployeeParam(Employee model, string roles)
    {
        return new EditEmployeeParam
        {
            EmployeeId = model.Id,
            Code = model.Code.Trim(),
            DomainUsername = model.DomainUsername,
            FullName = model.FullName,
            Nickname = model.NickName,
            EmployeeCard = model.EmployeeCard,
            DateOfBirth = model.DateOfBirth,
            Email = model.Email,
            JobTitleId = model.JobTitleId,
            JobLevelId = model.JobLevelId,
            ManagementLevelId = model.ManagementLevelId,
            Enabled = model.Enabled,
            Phone = model.Phone,
            Gender = model.Gender,
            Roles = roles,
            JoinedDate = model.JoinedDate,
            CompanyDate = model.CompanyDate,
            ConfirmedDate = model.ConfirmedDate,
            ResignedDate = model.ResignedDate,
            ResignedReason = model.ResignedReason,
            LetsTalk = model.LetsTalk,
            Marriage = model.Marriage,
            PermanentAddress = model.Address,
            CurrentAddress = model.CurrentAddress,
            IdCardNumber = model.CardNo,
            PassportNo = model.PassportNo,
            PassportExp = model.PassportExp,
            TeamId = model.Team.Id,
            IsLocal = model.IsLocal,
            CountryCode = model.CountryCode,
            CanOT = model.CanOT,
            SpecialOT = model.SpecialOT,
            WorkingDays = model.WorkingDays,
            WorkingHours = model.WorkingHours,
            WorkingHoursbyWeek = model.WorkingHoursbyWeek,
            IsTopManager = model.IsTopManager,
            OffDaysbyWeek = model.OffDaysbyWeek,
            PlaceOfBirth = model.PlaceOfBirth,
            Ethnic = model.Ethnic,
            Native = model.Native,
            Religion = model.Religion,
            DateOfIdCard = model.DateOfIdCard,
            PlaceOfIdCard = model.PlaceOfIdCard,
            HomePhone = model.HomePhone,
            Qualification = model.Qualification,
            Major = model.Major,
            IsExpatUseHoliday = !model.IsLocal && model.IsExpatUseHoliday,
            PickupRemark = model.PickupRemark,
            PromotionDate = model.PromotionDate,
            TeamDate = model.TeamDate,
            MilitaryFromDate = model.MilitaryFromDate,
            MilitaryToDate = model.MilitaryToDate,
            SubCompanyId = model.SubCompanyId,
            SalaryStructureId = model.SalaryStructureId,
            SpecialSubsidy = !string.IsNullOrWhiteSpace(model.SpecialSubsidy) ? Cryptography.EncryptPassword(model.SpecialSubsidy) : model.SpecialSubsidy,
            SpecialSubsidyCurrencyId = model.SpecialSubsidyCurrencyId.HasValue ? model.SpecialSubsidyCurrencyId.Value : null,
            AutoApproveRequest = model.AutoApproveRequest,
            AllowanceItemSettingIds = model.AllowanceItemSettingIds
        };
    }
}