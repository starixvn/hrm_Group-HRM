namespace HRM.Controllers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using Core;
using Core.Constants;
using HRM.Core.Models;
using HRM.Core.Services;
using Criteria.Employee;
using Criteria.EmployeeRelationship;
using Models;
using Models.EmployeeInformation;
using Resources;
using Services.Implement;
using Services.Interfaces;
using ViewModels.Information;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class InformationController(
    IUserControlSession userControlSession,
    IBaseService baseService,
    ICommonDataService commonDataService,
    IEmployeeService employeeService,
    IWebHostEnvironment hostingEnvironment,
    IEmailService emailService) : BaseController(userControlSession)
{
    private const string UpdateInfoNotificationTemplate = "EmailTemplates/UpdateInfoNotification.txt";

    public async Task<IActionResult> UpdateInformation()
    {
        var employee = await employeeService.GetEmployeeByIdAsync(EmployeeSession.EmpId);
        await DisplayDefaultValuesOfDropDownListsForPersonalInformation();
        ViewBag.BaseFileLocation = Constants.ProfileUploadPath + "/" + EmployeeSession.Code + "/";
        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateInformation(Employee model)
    {
        ModelState.Clear();
        model.Id = EmployeeSession.EmpId;
        var criteria = SetUpdatePersonalInfoCriteria(model);
        var employee = await employeeService.GetEmployeeByIdAsync(EmployeeSession.EmpId);

        var result = await baseService.ExecuteNonQueryAsync(criteria);

        if (result.IsSuccessful && IsValidUpdateFieldSendMail(model, employee))
        {
            await SendMailUpdateInfo(model, employee);
        }

        return Json(result);
    }

    private async Task DisplayDefaultValuesOfDropDownListsForPersonalInformation()
    {
        var countries = await commonDataService.GetAllCountryAsync();
        ViewBag.Countries = countries.Select(c => new SelectListItem { Text = c.CountryName, Value = c.CountryCode }).ToList();

        ViewBag.Genders = new List<SelectListItem>
        {
            new SelectListItem
            {
                Text = Labels.Female,
                Value = "0"
            },
            new SelectListItem
            {
                Text = Labels.Male,
                Value = "1"
            }
        };

        ViewBag.Marriages = Utilities.Marriages;
    }

    private static bool IsValidUpdateFieldSendMail(Employee model, Employee employee)
    {
        var isValidCardInfo = !IsEquals(employee.CardNo, model.CardNo)
                              || employee.DateOfIdCard != model.DateOfIdCard
                              || !IsEquals(employee.PlaceOfIdCard, model.PlaceOfIdCard);

        var isValidAddressInfo = !IsEquals(employee.Phone, model.Phone)
                                 || !IsEquals(employee.Address, model.Address)
                                 || !IsEquals(employee.CurrentAddress, model.CurrentAddress);

        return isValidCardInfo || isValidAddressInfo;
    }

    private static string GetTemplateChangeData(Employee model, Employee employee)
    {
        var template = new StringBuilder();
        var infoUpdate = new Dictionary<string, string>
        {
            { "CardNo", "- ID Card No.: {0} <br/>" },
            { "DateOfIdCard", "- ID Card Issue Date: {0} <br/>" },
            { "PlaceOfIdCard", "- ID Card Place Of Issue: {0} <br/>" },
            { "Phone", "- Cellphone: {0} <br/>" },
            { "Address", "- Permanent Address: {0} <br/>" },
            { "CurrentAddress", "- Current Address: {0} <br/>" }
        };

        foreach (var info in infoUpdate)
        {
            var oldValue = GetPropValue(employee, info.Key)?.ToString();
            var newValue = GetPropValue(model, info.Key)?.ToString();
            if (!IsEquals(oldValue, newValue))
            {
                template.AppendFormat(info.Value, GetPropValue(model, info.Key));
            }
        }

        return template.ToString();
    }

    private static object GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName).GetValue(src, null);
    }

    private static bool IsEquals(string oldValue, string newValue)
    {
        return string.IsNullOrWhiteSpace(oldValue) && string.IsNullOrWhiteSpace(newValue)
               || oldValue == newValue;
    }

    private async Task SendMailUpdateInfo(Employee model, Employee employee)
    {
        var hrAndSupervisors = await employeeService.GetHRAndSupervisorByEmpIdAsync(EmployeeSession.EmpId);
        var updateInfoTemplate = Utilities.ReadTextFile(Path.Combine(hostingEnvironment.ContentRootPath, UpdateInfoNotificationTemplate));

        foreach (var hrAndSupervisor in hrAndSupervisors)
        {
            if (!string.IsNullOrWhiteSpace(hrAndSupervisor.Email))
            {
                var body = string.Format(
                    updateInfoTemplate,
                    hrAndSupervisor.NickName,
                    employee.NickName,
                    employee.FullName,
                    GetTemplateChangeData(model, employee)
                );

                await emailService.SendEmailAsync(
                    hrAndSupervisor.Email,
                    string.Empty,
                    $"Updated Personal Information from {employee.NickName} ({employee.FullName})",
                    body
                );
            }
        }
    }

    private UpdateEmployeePersonalInformationCriteria SetUpdatePersonalInfoCriteria(Employee employee)
    {
        var criteria = new UpdateEmployeePersonalInformationCriteria
        {
            EmpId = employee.Id,
            Email = employee.Email,
            DateOfBirth = employee.DateOfBirth,
            PlaceOfbirth = employee.PlaceOfBirth,
            Ethnic = employee.Ethnic,
            Native = employee.Native,
            Religion = employee.Religion,
            DateOfIdCard = employee.DateOfIdCard,
            PlaceOfIdCard = employee.PlaceOfIdCard,
            HomePhone = employee.HomePhone,
            Qualification = employee.Qualification,
            Major = employee.Major,
            Phone = employee.Phone,
            Gender = employee.Gender,
            Skype = employee.Skype,
            LetsTalk = employee.LetsTalk,
            ChineseName = employee.ChineseName,
            Marriage = employee.Marriage,
            PermanentAddress = employee.Address,
            CurrentAddress = employee.CurrentAddress,
            IdCardNumber = employee.CardNo,
            PassportNo = employee.PassportNo,
            PassportExp = employee.PassportExp,
            ModifiedBy = EmployeeSession.EmpId,
            CountryCode = employee.CountryCode
        };

        return criteria;
    }

    public ActionResult GetFileUploaded()
    {
        var fileNames = GetAllFileUploaded();
        ViewBag.BaseUrl = $"/{Constants.ProfileUploadPath}/{EmployeeSession.Code}/";
        return PartialView("_ListFileUploaded", fileNames);
    }

    private IList<string> GetAllFileUploaded()
    {
        var filePath = new List<string>();
        var path = Path.Combine(hostingEnvironment.ContentRootPath, Constants.ProfileUploadPath + EmployeeSession.Code); ;
        if (Directory.Exists(path))
        {
            var paths = Directory.GetFiles(path);
            foreach (var p in paths)
            {
                filePath.Add(Path.GetFileName(p));
            }
        }

        return filePath;
    }

    public async Task<IActionResult> GetEmployeeRelationship()
    {
        var criteria = new GetRelationshipByEmployeeIdCriteria
        {
            EmployeeId = EmployeeSession.EmpId
        };

        var relationships = await baseService.GetDataAsync<EmployeeRelationship>(criteria);

        return PartialView("_ListRelationship", relationships);
    }

    public ActionResult InsertRelationship()
    {
        var viewModel = new EmployeeRelationshipViewModel
        {
            BirthDay = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime
        };

        return PartialView("_AddRelationship", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> InsertRelationship(EmployeeRelationshipViewModel viewModel)
    {
        var result = new CommandResult { Code = 1, Message = string.Empty };

        if (ModelState.IsValid)
        {
            var criteria = new InsertRelationshipCriteria
            {
                EmployeeId = EmployeeSession.EmpId,
                FullName = viewModel.FullName,
                Gender = viewModel.Gender,
                DateOfBirth = viewModel.BirthDay,
                Phone = viewModel.Phone,
                Relationship = viewModel.Relationship,
                IsEmergency = viewModel.IsEmergency,
                Creator = EmployeeSession.EmpId
            };

            result = await baseService.ExecuteNonQueryAsync(criteria);
        }
        else
        {
            result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
        }

        return Json(result);
    }

    public async Task<IActionResult> EditRelationship(int id)
    {
        var criteria = new GetRelationshipByIdCriteria
        {
            RelationshipId = id
        };
        var relationship = await baseService.GetDataEntityAsync<EmployeeRelationship>(criteria);
        var viewModel = new EmployeeRelationshipViewModel
        {
            FullName = relationship.FullName,
            Gender = relationship.Gender,
            Phone = relationship.Phone,
            Relationship = relationship.Relationship,
            BirthDay = relationship.DateOfBirth,
            IsEmergency = relationship.IsEmergency
        };
        return PartialView("_EditRelationship", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditRelationship(int id, EmployeeRelationshipViewModel viewModel)
    {
        var result = new CommandResult { Code = 1, Message = string.Empty };
        if (ModelState.IsValid)
        {
            var criteria = new EditRelationshipCriteria
            {
                RelationshipId = id,
                FullName = viewModel.FullName,
                Gender = viewModel.Gender,
                DateOfBirth = viewModel.BirthDay,
                Phone = string.IsNullOrEmpty(viewModel.Phone) ? string.Empty : viewModel.Phone,
                Relationship = viewModel.Relationship,
                IsEmergency = viewModel.IsEmergency,
                Modifier = EmployeeSession.EmpId
            };

            result = await baseService.ExecuteNonQueryAsync(criteria);
        }
        else
        {
            result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
        }

        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRelationship(int id)
    {
        var criteria = new DeleteRelationshipCriteria
        {
            RelationshipId = id,
            ActorId = EmployeeSession.EmpId
        };

        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return Json(result);
    }
}