namespace HRM.Core.Models;

using System;

[Serializable]
public class EmployeeSession
{
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string FullName { get; set; }
    public string Nickname { get; set; }
    public string Password { get; set; }
    public bool Enabled { get; set; }
    public bool IsTopManager { get; set; }
    public string DisplayName => string.IsNullOrWhiteSpace(Nickname) ? FullName : Nickname + " (" + FullName + ")";
    public int CompanyId { get; set; }
}