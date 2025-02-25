namespace HRM.AutoJobs.Models
{
    using System;

    [Serializable]
    public class EmployeeInfo
    {
        public int EmpId { get; set; }
        public int? UserId { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public string TeamCode { get; set; }
        public string TeamName { get; set; }
        public DateTime? DoB { get; set; }
        public byte Gender { get; set; }
        public string JobTitle { get; set; }
        public string JobLevel { get; set; }
        public DateTime? JoinedDate { get; set; }
        public DateTime? CompanyDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public DateTime? ResignedDate { get; set; }
        public DateTime? MilitaryFromDate { get; set; }
        public DateTime? MilitaryToDate { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Nationality { get; set; }
        public bool IsLocal { get; set; }
        public bool IsSpecialUser { get; set; }
        public bool Enabled { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}