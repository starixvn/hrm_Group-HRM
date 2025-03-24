using System;

namespace HRM.Models.FnB;

public class SyncFnBRequestModel
{
    public short CompanyId { get; set; }

    public DateTime LastModifiedDate { get; set; }
}