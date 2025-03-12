namespace HRM.ViewModels;

using System;
using System.Collections.Generic;

[Serializable]
public class MultiSelectViewModel
{
    public int Id { get; set; }
    public string Text { get; set; }
    public bool Checked { get; set; }
    public bool Enable { get; set; } = true;
    public int? ParentId { get; set; }
    public bool IsLeaf { get; set; }
    public IList<MultiSelectViewModel> Items { get; set; } = new List<MultiSelectViewModel>();
}