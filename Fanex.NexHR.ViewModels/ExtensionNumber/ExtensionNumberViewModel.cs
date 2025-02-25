namespace Fanex.NexHR.ViewModels.ExtensionNumber
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class ExtensionNumberViewModel
    {
        public string Name { get; set; }

        public IList<ExtensionNoModel> Extensions { get; set; }
    }
}
