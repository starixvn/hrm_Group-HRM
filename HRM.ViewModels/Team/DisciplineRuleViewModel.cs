using HRM.Models.Group;
using System.Collections.Generic;

namespace HRM.ViewModels.Team;

public class DisciplineRuleViewModel
{
    public int TeamId { get; set; }
    public string Team { get; set; }
    public bool IsHost { get; set; }
    public IEnumerable<DisciplineRule> DisciplineRules { get; set; } = new List<DisciplineRule>();
}