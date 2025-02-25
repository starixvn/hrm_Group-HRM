namespace HRM.ViewModels.Function
{
    using System.Collections.Generic;
    using GO.UserControl.Client.Core.Models;

    public class TreeFunctionViewModel
    {
        public short RoleId { get; set; }
        public bool IsHost { get; set; }
        public IList<Function> Functions { get; set; } = new List<Function>();
        public IList<Function> HierarchyFunctions { get; set; } = new List<Function>();
        public IList<FunctionPermission> Permissions { get; set; } = new List<FunctionPermission>();
    }
}