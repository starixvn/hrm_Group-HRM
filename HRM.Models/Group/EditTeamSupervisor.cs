namespace HRM.Models.Group;

using System;
using Core.XMLExtensions;

[Serializable]
[XMLName("Row")]
public class EditTeamSupervisor
{
	public ushort RequestType { get; set; }
}