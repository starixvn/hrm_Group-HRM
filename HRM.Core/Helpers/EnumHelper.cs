using Fanex.Common.Extensions;

namespace HRM.Core.Helpers
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    public static class EnumHelper
    {
        public static string GetDescription(Enum enumType)
        {
            var description = string.Empty;
            if (enumType != null)
            {
                var fi = enumType.GetType().GetField(enumType.ToString());
                var attributes = (DescriptionAttribute[])fi?.GetCustomAttributes(typeof(DescriptionAttribute), false);
                description = attributes?.Length > 0 ? attributes[0].Description : enumType.ToString();
            }

            return description;
        }

        public static string GetDisplayName(this Enum val)
        {
            return val.GetType().GetMember(val.ToString())
                .FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>()?.GetName()
                ?? val.ToString();
        }

        public static string GetStringValue(Enum enumType)
        {
            var description = string.Empty;
            if (enumType == null)
            {
                return description;
            }
            var fi = enumType.GetType().GetField(enumType.ToString());
            if (fi == null)
            {
                return description;
            }
            var attributes = (StringValueAttribute[])fi.GetCustomAttributes(typeof(StringValueAttribute), false);
            description = attributes.Length > 0 ? attributes[0].Value : enumType.ToString();

            return description;
        }
    }
}