using System.Collections.Generic;
using System.Linq;

namespace HRM.Models.Report.HeadcountReport;

public class ReportModel
{
    public int? Month { get; set; }
    public int? NewHire => NewHireEmpIds.Count();
    public int? ProbationResign => ProbationResignEmpIds.Count();
    public int? ConfirmedResign => ConfirmedResignEmpIds.Count();
    public int? Transfer => TransferInEmpIds.Count() - TransferOutEmpIds.Count();
    public int? CurrentHeadCount => CurrentHeadCountEmpIds.Count();
    
    public IEnumerable<int> NewHireEmpIds { get; set; } = [];
    public IEnumerable<int> ProbationResignEmpIds { get; set; } = [];
    public IEnumerable<int> ConfirmedResignEmpIds { get; set; } = [];
    public IEnumerable<int> TransferInEmpIds { get; set; } = [];
    public IEnumerable<int> TransferOutEmpIds { get; set; } = [];
    public IEnumerable<int> CurrentHeadCountEmpIds { get; set; } = [];
}