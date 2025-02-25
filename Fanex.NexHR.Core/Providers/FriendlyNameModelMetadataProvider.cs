namespace Fanex.NexHR.Core.Providers
{
    using System;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;

    public class FriendlyNameModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata GetMetadataForProperty(
            Func<object> modelAccessor,
            Type containerType,
            PropertyDescriptor propertyDescriptor)
        {
            ModelMetadata metadata = base.GetMetadataForProperty(modelAccessor, containerType, propertyDescriptor);

            metadata.DisplayName = metadata.DisplayName ?? DisplayNameFromCamelCase(metadata.GetDisplayName());

            return metadata;
        }

        private static string DisplayNameFromCamelCase(string name)
        {
            const string temp = "(?<!^)" + // don't match on the first character - never want to place a space here
            "(" +
            "  [A-Z][a-z] |" + // put a space before "Aaaa"
            "  (?<=[a-z])[A-Z] |" + // put a space into "aAAA" before the first capital
            "  (?<![A-Z])[A-Z]$" + // if the last letter is capital, prefix it with a space too
            ")";

            var result = Regex.Replace(
                name,
                temp,
                " $1",
                RegexOptions.IgnorePatternWhitespace).Trim();

            return result.EndsWith(" Id", StringComparison.OrdinalIgnoreCase) ? result.Replace(" Id", string.Empty) : result;
        }
    }
}