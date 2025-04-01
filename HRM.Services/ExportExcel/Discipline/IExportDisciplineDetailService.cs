using System.Collections.Generic;
using HRM.Criteria.Discipline;
using HRM.Models.Discipline;

namespace HRM.Services.ExportExcel.Discipline;

public interface IExportDisciplineDetailService
{
    byte[] Export(GetDisciplineDetailCriteria criteria, IList<DisciplineDetailModel> disciplines);
}