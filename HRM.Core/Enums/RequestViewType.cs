namespace HRM.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum RequestViewType
    {
        [StringValue("Personal")]
        PersonalSpace = 1,

        [StringValue("HR Space")]
        HRSpace = 2,

        [StringValue("Company Space")]
        CompanySpace = 3,

        [StringValue("SA Space")]
        SASpace = 4
    }
}