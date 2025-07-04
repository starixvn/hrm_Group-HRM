using HRM.Core.Constants;

namespace HRM.Services.Implement;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Criteria.Department;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Repositories.Interfaces;
using Interfaces;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Models;
using OfficeOpenXml;

public class SalaryStructureService(
    IDepartmentRepository departmentRepository,
    IBaseService baseService,
    IWebHostEnvironment hostingEnvironment) : ISalaryStructureService
{

    public async Task<CommandResult> DeleteAsync(int actorId, int id)
    {
        var criteria = new DeleteSalaryStructureCriteria
        {
            SalaryStructureId = id,
            ModifiedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<IList<SalaryStructure>> GetSalaryStructureAsync()
    {
        var criteria = new GetSalaryStructureCriteria();

        return (await baseService.GetDataAsync<SalaryStructure>(criteria)).ToList();
    }

    public async Task<SalaryStructure> GetByIdAsync(int salaryStructureId)
    {
        var criteria = new GetSalaryStructureByIdCriteria
        {
            Id = salaryStructureId
        };

        return await baseService.(criteria);
    }
}