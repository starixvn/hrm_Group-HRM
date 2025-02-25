namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using HRM.Core;
    using HRM.Core.Enums;
    using HRM.Models;
    using HRM.Models.EmployeeInformation;
    using HRM.Models.Supervisor;
    using HRM.Resources;

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportExcelEmployeeInfoService : IExportExcelEmployeeInfoService
    {
        private const string EmployeeBasicInfoTemplatePath = @"~\ExcelTemplates\EmployeeBasicInfoTemplate.xls";
        private const string EmployeeDetailsInfoTemplatePath = @"~\ExcelTemplates\EmployeeDetailsInfoTemplate.xls";
        private const string EmployeeFullInfoTemplatePath = @"~\ExcelTemplates\EmployeeFullProfileInfoTemplate.xls";
        private readonly IEmployeeService employeeService;
        private readonly ICommonDataService commonDataService;

        public ExportExcelEmployeeInfoService(
            IEmployeeService employeeService,
            ICommonDataService commonDataService)
        {
            this.employeeService = employeeService;
            this.commonDataService = commonDataService;
        }

        private enum BasicInfo
        {
            RowStartHeader = 2,
            RowStartBody = 4,
            TotalColumns = 22
        }

        public void ExportEmployeeBasicInfo(IList<Employee> employees)
        {
            string templatePath = HttpContext.Current.Server.MapPath(EmployeeBasicInfoTemplatePath);
            string fileName = "EmployeeBasicInfo_" + DateTimeOffset.Now.ToString("yyyyMMdd");
            const string sheetName = "Employee Basic Info";
            var writer = new ExcelTemplateWriter(templatePath, fileName, sheetName);
            bool enableGroupDate = commonDataService.IsEnableGroupDate();
            const byte colGroupDate = 8;
            const byte colLengthOfServiceGroupDate = 12;

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
            writer.CopyRows((byte)BasicInfo.RowStartHeader, 2);

            FillContentRowEmployeeBasicInfoReport(employees, ref writer);
            writer.SyncColumnsWidth((byte)BasicInfo.TotalColumns);
            writer.CreateFreezePane(0, (byte)BasicInfo.RowStartBody);
            if (!enableGroupDate)
            {
                writer.RemoveColumn(colGroupDate);
                writer.RemoveColumn(colLengthOfServiceGroupDate);
            }
            writer.Render();
        }

        public void ExportEmployeeDetailsInfo(IList<Employee> employees)
        {
            string templatePath = HttpContext.Current.Server.MapPath(EmployeeDetailsInfoTemplatePath);
            string fileName = "EmployeeDetailsInfo_" + DateTimeOffset.Now.ToString("yyyyMMdd");
            const string sheetName = "Employee Details Info";
            var writer = new ExcelTemplateWriter(templatePath, fileName, sheetName);

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
            writer.CopyRows(2, 1);

            FillContentRowEmployeeDetailsInfoReport(employees, ref writer);
            writer.SyncColumnsWidth(43);
            writer.CreateFreezePane(0, 3);
            writer.Render();
        }

        #region Employee Basic Info

        private void FillContentRowEmployeeBasicInfoReport(IList<Employee> employees, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (Employee employee in employees)
            {
                FillRowEmployeeBasicInfoReport(employee, ref writer, ref rowIndex);
            }
        }

        private void FillRowEmployeeBasicInfoReport(Employee employee, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordEmployeeBasicInfoReport(employee, rowIndex);

            var writeRowNumber = (rowIndex % 2 == 0) ? 5 : 4;

            writer.WriteRow(writeRowNumber, record);

            rowIndex++;
        }

        private object CreateRecordEmployeeBasicInfoReport(Employee employee, int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                employee.UnitName,
                Department = employee.Team.Section.Department.Name,
                Section = employee.Team.Section.Name,
                Team = employee.Team.Name,
                employee.Code,
                employee.FullName,
                employee.NickName,
                GroupDate = employee.JoinedDate,
                employee.CompanyDate,
                employee.ConfirmedDate,
                employee.ResignedDate,
                employee.ResignedReason,
                LengthOfServiceByGroupDate = employeeService.CaculateLengthOfService(employee.JoinedDate, employee.MilitaryFromDate, employee.MilitaryToDate),
                LengthOfServiceByCompanyDate = employeeService.CaculateLengthOfService(employee.CompanyDate, employee.MilitaryFromDate, employee.MilitaryToDate),
                DoB = employee.DateOfBirth.ToDateMonthYearFormatedDate(),
                Gender = employee.Gender == 0 ? "F" : "M",
                CellPhone = employee.Phone,
                employee.Email,
                employee.Address,
                employee.CurrentAddress,
                Category = employee.IsLocal ? Labels.Local : Labels.Expat
            };

            return record;
        }

        #endregion Employee Basic Info

        #region Employee Details Info

        private void FillContentRowEmployeeDetailsInfoReport(IList<Employee> employees, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (Employee employee in employees)
            {
                FillRowEmployeeDetailsInfoReport(employee, ref writer, ref rowIndex);
            }
        }

        private void FillRowEmployeeDetailsInfoReport(Employee employee, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordEmployeeDetailsInfoReport(employee, rowIndex);
            var writeRowNumber = (rowIndex % 2 == 0) ? 4 : 3;

            writer.WriteRow(writeRowNumber, record);

            rowIndex++;
        }

        private object CreateRecordEmployeeDetailsInfoReport(Employee employee, int rowIndex)
        {
            const string DateFormat = "dd MMM, yyyy";
            var record = new
            {
                Count = rowIndex,
                Code = employee.Code,
                EmployeeCard = employee.EmployeeCard,
                DomainUsername = employee.DomainUsername,
                FullName = employee.FullName,
                NickName = employee.NickName,
                Team = employee.Team.Name,
                DoB = employee.DateOfBirth,
                PlaceOfBirth = employee.PlaceOfBirth,
                Ethnic = employee.Ethnic,
                Native = employee.Native,
                Religion = employee.Religion,
                Gender = employee.Gender == 0 ? "F" : "M",
                MarriageName = employee.MarriageName,
                CountryName = employee.CountryName,
                CardNo = employee.CardNo,
                DateOfIdCard = FormatDate(employee.DateOfIdCard, DateFormat),
                PlaceOfIdCard = employee.PlaceOfIdCard,
                PassportNo = employee.PassportNo,
                PassportExp = FormatDate(employee.PassportExp, DateFormat),
                Qualification = employee.Qualification,
                Major = employee.Major,
                Phone = employee.Phone,
                HomePhone = employee.HomePhone,
                Email = employee.Email,
                Address = employee.Address,
                CurrentAddress = employee.CurrentAddress,
                Skype = employee.Skype,
                Unit = employee.UnitName,
                JobTitle = employee.JobTitle,
                GroupDate = FormatDate(employee.JoinedDate, DateFormat),
                CompanyDate = FormatDate(employee.CompanyDate, DateFormat),
                ConfirmedDate = FormatDate(employee.ConfirmedDate, DateFormat),
                ResignedDate = FormatDate(employee.ResignedDate, DateFormat),
                ResignReason = employee.ResignedReason,
                WorkingDays = employee.WorkingDays,
                WorkingHoursbyWeek = employee.WorkingHoursbyWeek,
                WorkingHours = employee.WorkingHours,
                OffDaysbyWeek = employee.OffDaysbyWeek,
                AnnualLeaveBalance = employee.AnnualLeaveBalance,
                Category = employee.IsLocal ? Labels.Local : Labels.Expat,
                BankAccount = employee.BankAccount,
                TaxCode = employee.TaxCode,
                SocialInsuranceNo = employee.SocialInsuranceNo,
                RegisteredHospital = employee.RegisteredHospital
            };

            return record;
        }

        #endregion Employee Details Info

        #region Employee Full Profile Information

        public void ExportEmployeeFullProfile(IList<EmployeeFullProfile> profiles)
        {
            string templatePath = HttpContext.Current.Server.MapPath(EmployeeFullInfoTemplatePath);
            string fileName = "EmployeeFullInfo_" + DateTimeOffset.Now.ToString("yyyyMMdd");
            const string sheetName = "Employee Full Information";
            var writer = new ExcelTemplateWriter(templatePath, fileName, sheetName);
            bool enableGroupDate = commonDataService.IsEnableGroupDate();
            const byte colGroupDate = 10;
            const byte colLengthOfServiceGroupDate = 14;

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
            writer.CopyRows(2, 2);

            WriteFullProfileBody(writer, profiles);
            writer.SyncColumnsWidth(57);
            writer.CreateFreezePane(0, 4);
            if (!enableGroupDate)
            {
                writer.RemoveColumn(colGroupDate);
                writer.RemoveColumn(colLengthOfServiceGroupDate);
            }
            writer.Render();
        }

        private void WriteFullProfileBody(ExcelTemplateWriter writer,
            IList<EmployeeFullProfile> profiles)
        {
            var currentRow = 3;
            foreach (var p in profiles)
            {
                var writeRowNumber = (currentRow % 2 == 0) ? 5 : 4;

                var record = CreateRecordEployeeFullProfile(p, currentRow);
                writer.WriteRow(writeRowNumber, record);
                currentRow++;
            }
        }

        private object CreateRecordEployeeFullProfile(EmployeeFullProfile p, int currentRow)
        {
            var gender = "Unknown";
            if (p.Gender.HasValue)
            {
                gender = p.Gender == 0 ? "Female" : "Male";
            }

            var record = new
            {
                Index = currentRow - 2,
                p.UnitName,
                Department = p.DepartmentName,
                Section = p.SectionName,
                p.TeamName,
                p.Code,
                p.EmployeeCard,
                p.FullName,
                p.Nickname,
                Gender = gender,
                GroupDate = FormatDate(p.JoinedDate),
                CompanyDate = FormatDate(p.CompanyDate),
                ConfirmedDate = FormatDate(p.ConfirmedDate),
                ResignedDate = FormatDate(p.ResignedDate),
                p.ResignedReason,
                LengthOfServiceByGroupDate = employeeService.CaculateLengthOfService(p.JoinedDate, p.MilitaryFromDate, p.MilitaryToDate),
                LengthOfServiceByCompanyDate = employeeService.CaculateLengthOfService(p.CompanyDate, p.MilitaryFromDate, p.MilitaryToDate),
                p.JobTitle,
                p.JobLevel,
                TeamDate = FormatDate(p.TeamDate),
                DOB = FormatDate(p.DateOfBirth),
                p.PlaceOfBirth,
                p.Ethnic,
                p.Native,
                p.Religion,
                p.MarriageName,
                p.Nationality,
                p.CardNo,
                DateOfIDCard = FormatDate(p.DateOfIdCard),
                p.PlaceOfIdCard,
                p.PassportNo,
                PassportExp = FormatDate(p.PassportExp),
                p.Qualification,
                p.Major,
                p.Address,
                p.CurrentAddress,
                p.Phone,
                p.HomePhone,
                p.Email,
                p.PickupRemark,
                p.Skype,
                p.LetsTalk,
                CanOT = p.CanOT ? Labels.Yes : Labels.No,
                p.SpecialOT,
                p.WorkingDays,
                p.WorkingHours,
                p.OffDaysbyWeek,
                p.AnnualLeaveBalance,
                p.KeepInViewBalance,
                Category = p.IsLocal ? Labels.Local : Labels.Expat,
                p.Roles2String,
                SpecialUser = p.IsTopManager ? Labels.Yes : Labels.No,
                VerifiedBy = GetSupervisors(p.Supervisors, (short)RoleSystem.Verifier),
                ApprovedBy = GetSupervisors(p.Supervisors, (short)RoleSystem.Approver),
                DisciplineManager = GetSupervisors(p.Supervisors, (short)RoleSystem.DisciplineManager),
                ShiftArrangement = GetSupervisors(p.Supervisors, (short)RoleSystem.ShiftManager),
                OnBehalfRequest = GetSupervisors(p.Supervisors, (short)RoleSystem.OnBehalfRequest),
            };

            return record;
        }

        private static string GetSupervisors(IList<SupervisorNameModel> supervisors, short roleId)
        {
            var nicknames = string.Empty;
            if (supervisors != null && supervisors.Any())
            {
                nicknames = string.Join(", ", supervisors.Where(x => x.ActionType == roleId).Select(x => x.NickName));
            }

            return nicknames;
        }

        private static string FormatDate(DateTime? date, string format = "dd/MM/yyyy") => date?.ToString(format);

        #endregion Employee Full Profile Information
    }
}