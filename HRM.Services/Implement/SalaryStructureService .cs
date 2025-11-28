using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.SalaryStructure;
using HRM.Criteria.SystemSetting;
using HRM.Models.SalaryStructure;
using HRM.Services.Interfaces;
using HRM.ViewModels.SalaryStructure;

namespace HRM.Services.Implement;

public class SalaryStructureService(IBaseService baseService) : ISalaryStructureService
{
    public async Task<IEnumerable<SalaryStructure>> GetSalaryStructureAsync(int companyId)
    {
        var criteria = new GetSalaryStructureCriteria
        {
            CompanyId = companyId
        };
        var (salaryStructures, details) = await baseService.FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(criteria);
        foreach (var item in details)
        {
            item.BasicSalary = !string.IsNullOrWhiteSpace(item.BasicSalary)
                ? Cryptography.DecryptPassword(item.BasicSalary)
                : null;

            item.JobLevelAddOn = !string.IsNullOrWhiteSpace(item.JobLevelAddOn)
                ? Cryptography.DecryptPassword(item.JobLevelAddOn)
                : null;

            item.ExpatSalary = !string.IsNullOrWhiteSpace(item.ExpatSalary)
                ? Cryptography.DecryptPassword(item.ExpatSalary)
                : null;

            item.ExpatJobLevelAddOn = !string.IsNullOrWhiteSpace(item.ExpatJobLevelAddOn)
                ? Cryptography.DecryptPassword(item.ExpatJobLevelAddOn)
                : null;
        }

        foreach (var structure in salaryStructures)
        {
            structure.Details = details.Where(x => x.SalaryStructureId == structure.SalaryStructureId);
        }

        return salaryStructures;
    }

    public async Task<SalaryStructure> GetByIdAsync(int salaryStructureId)
    {
        var criteria = new GetSalaryStructureByIdCriteria
        {
            SalaryStructureId = salaryStructureId
        };
        var (salaryStructures, details) = await baseService.FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(criteria);
        var salaryStructure = salaryStructures.FirstOrDefault();
        if (salaryStructure != null)
        {
            foreach (var item in details)
            {
                item.BasicSalary = !string.IsNullOrWhiteSpace(item.BasicSalary)
                    ? Cryptography.DecryptPassword(item.BasicSalary)
                    : null;

                item.JobLevelAddOn = !string.IsNullOrWhiteSpace(item.JobLevelAddOn)
                    ? Cryptography.DecryptPassword(item.JobLevelAddOn)
                    : null;

                item.ExpatSalary = !string.IsNullOrWhiteSpace(item.ExpatSalary)
                    ? Cryptography.DecryptPassword(item.ExpatSalary)
                    : null;

                item.ExpatJobLevelAddOn = !string.IsNullOrWhiteSpace(item.ExpatJobLevelAddOn)
                    ? Cryptography.DecryptPassword(item.ExpatJobLevelAddOn)
                    : null;
            }

            salaryStructure.Details = details;
        }

        return salaryStructure;
    }

    public async Task<CommandResult> DeleteAsync(int actorId, int id)
    {
        var criteria = new DeleteSalaryStructureCriteria
        {
            SalaryStructureId = id,
            DeletedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<BasicSalaryConfigViewModel> GetBasicSalaryConfigAsync(short companyId)
    {
        var criteria = new GetSystemConfigsByKeyCriteria
        {
            CompanyId = companyId,
            Key = SystemConfigKey.BasicSalaryConfig,
        };

        var config = await baseService.GetDataEntityAsync<string>(criteria);

        var configOptions = new List<BasicSalaryConfigOption>
        {
            new() { Value = "1", Label = "Basic Salary" },
            new() { Value = "2", Label = "Job Level Add-On" },
            new() { Value = "3", Label = "Special Subsidy" },
            new() { Value = "4", Label = "Management Allowance" }
        };

        return new BasicSalaryConfigViewModel
        {
            Config = config,
            ConfigOptions = configOptions
        };
    }
}
