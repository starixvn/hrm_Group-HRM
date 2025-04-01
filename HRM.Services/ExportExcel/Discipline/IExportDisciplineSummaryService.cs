using System.Collections.Generic;
using HRM.Criteria.Discipline;
using HRM.Models.Discipline;

namespace HRM.Services.ExportExcel.Discipline;

public interface IExportDisciplineSummaryService
{
    byte[] Export(GetDisciplineSummaryCriteria criteria, IList<DisciplineSummaryModel> disciplines);
}