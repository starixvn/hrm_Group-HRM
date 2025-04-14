using System;

namespace HRM.Models.FnB;

public class TeamModel
{
    public int TeamId { get; set; }

    public int SectionId { get; set; }
        
    public string Code { get; set; }
        
    public string Name { get; set; }

    public int LeaderId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime ModifiedDate { get; set; }
}