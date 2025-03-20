namespace HRM.Criteria.Employee;

using System;
using Fanex.Data;
using Fanex.Data.Repository;

public class UpdateEmployeePersonalInformationCriteria : CriteriaBase
{
    public int EmpId { get; set; }

    public string Email { get; set; }

    [SpParam(SqlParamName = "DOB")]
    public DateTime? DateOfBirth { get; set; }

    public string Phone { get; set; }

    public byte? Gender { get; set; }

    public string Skype { get; set; }

    public string LetsTalk { get; set; }

    public byte Marriage { get; set; }

    public string PermanentAddress { get; set; }

    public string CurrentAddress { get; set; }

    public string IdCardNumber { get; set; }

    public string PassportNo { get; set; }

    public DateTime? PassportExp { get; set; }

    public string ChineseName { get; set; }

    public int ModifiedBy { get; set; }

    public string CountryCode { get; set; }

    public string PlaceOfbirth { get; set; }

    public string Ethnic { get; set; }

    public string Native { get; set; }

    public string Religion { get; set; }

    public DateTime? DateOfIdCard { get; set; }

    public string PlaceOfIdCard { get; set; }

    public string HomePhone { get; set; }

    public string Qualification { get; set; }

    public string Major { get; set; }

    public override string GetSettingKey() => "Employee_Information_UpdateByEmpId";

    public override bool IsValid() => EmpId > 0;
}