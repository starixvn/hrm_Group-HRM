namespace HRM.Models.Report;

public class ManpowerCategoryModel
{
    public string DepartmentCode { get; set; }
    public string Section { get; set; }
    public string TeamCode { get; set; }

    public string TeamName { get; set; }

    public int Total => TotalExpat + TotalLocal;

    public int TotalExpat { get; set; }

    public int TotalLocal { get; set; }

    public int Management { get; set; }
    public double AVGManagement => Total != 0 ? (Management * 1.00) / Total : 0;

    public int ExecutiveWithPosition { get; set; }
    public double AVGExecutiveWithPosition => Total != 0 ? (ExecutiveWithPosition * 1.00) / Total : 0;

    public int Executive { get; set; }
    public double AVGExecutive => Total != 0 ? (Executive * 1.00) / Total : 0;

    public int Staff { get; set; }
    public double AVGStaff => Total != 0 ? (Staff * 1.00) / Total : 0;

    public int Worker { get; set; }
    public double AVGWorker => Total != 0 ? (Worker * 1.00) / Total : 0;
}