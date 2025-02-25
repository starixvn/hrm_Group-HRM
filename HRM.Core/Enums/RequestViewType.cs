namespace HRM.Core.Enums
{
    using System.ComponentModel;

    public enum RequestViewType
    {
        [Description("Personal")]
        PersonalSpace = 1,

        [Description("HR Space")]
        HRSpace = 2,

        [Description("Company Space")]
        CompanySpace = 3,

        [Description("SA Space")]
        SASpace = 4
    }
}