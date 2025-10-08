namespace HRM.Models.Common;

public abstract class MonthlyDataBase
{
    public float Jan { get; set; }
    public float Feb { get; set; }
    public float Mar { get; set; }
    public float Apr { get; set; }
    public float May { get; set; }
    public float Jun { get; set; }
    public float Jul { get; set; }
    public float Aug { get; set; }
    public float Sep { get; set; }
    public float Oct { get; set; }
    public float Nov { get; set; }
    public float Dec { get; set; }

    public float Total => Jan + Feb + Mar + Apr + May + Jun + Jul + Aug + Sep + Oct + Nov + Dec;
}
