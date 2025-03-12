namespace HRM.ViewModels.ExtensionNumber
{
    using System.Collections.Generic;
    using Models;

    public class ExtensionNumberViewModel
    {
        public string Name { get; set; }

        public IList<ExtensionNoModel> Extensions { get; set; }
    }
}
