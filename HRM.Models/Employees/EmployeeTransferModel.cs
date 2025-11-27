namespace HRM.Models.Employees;

using System;

public class EmployeeTransferModel
{
    // Employee Information
    public string CtrId { get; set; }
    public string CtrIdFormatted => CtrId is not null ? CtrId.PadLeft(6, '0') : string.Empty;
    public string FullName { get; set; }

    // From Company Information
    public string FromCompanyName { get; set; }
    public string FromNickName { get; set; }
    public string FromBUName { get; set; }
    public string FromDeptName { get; set; }
    public string FromTeamName { get; set; }

    // To Company Information
    public string ToCompanyName { get; set; }
    public string ToNickName { get; set; }
    public string ToBUName { get; set; }
    public string ToDeptName { get; set; }
    public string ToTeamName { get; set; }

    // Transfer Information
    public string Remark { get; set; }
    public DateTime? TransferredDatetime { get; set; }
    public string TransferredByNickName { get; set; }
    public string TransferredByCompany { get; set; }
}

