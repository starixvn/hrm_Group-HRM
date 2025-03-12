namespace HRM.Core.XMLExtensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

public static class XMLIO
{
    public static XElement ToXML<T>(this IEnumerable<T> source)
    {
        var lambda = GetElement<T>(typeof(T));

        if(source == null || !source.Any())
        {
            return new XElement("Root");
        }

        return new XElement("Root", source.Select(lambda).ToArray());
    }

    private static XName GetElementName(Type t)
    {
        var attr = t.GetCustomAttribute<XMLNameAttribute>();
        return attr == null ? t.Name : attr.Name;
    }

    private static XName GetPropertyName(PropertyInfo p)
    {
        var attr = p.GetCustomAttribute<XMLNameAttribute>();
        return attr == null ? p.Name : attr.Name;
    }

    private static Func<T, XElement> GetElement<T>(Type t)
    {
        Expression<Func<XName, object, XAttribute>> attrSetter =
            (name, obj) => new XAttribute(name, obj == null ? string.Empty : obj.ToString().Trim());
        var elementName = GetElementName(t);
        var p = Expression.Parameter(t);

        var args = GetExpressions(t, p, attrSetter);

        var constructor = typeof(XElement).GetConstructor(new[] { typeof(XName), typeof(object[]) });
        var body = Expression.New(constructor, Expression.Constant(elementName), Expression.NewArrayInit(typeof(object), args));

        return Expression.Lambda<Func<T, XElement>>(body, p).Compile();
    }

    private static IEnumerable<Expression> GetExpressions(Type t, Expression obj, Expression lambda)
    {
        var result = t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.GetCustomAttribute<XmlIgnoreAttribute>() == null)
            .SelectMany(p => GetExpressions(p, obj, lambda));
        return result;
    }

    private static IEnumerable<Expression> GetExpressions(PropertyInfo p, Expression obj, Expression lambda)
    {
        var prop = Expression.Property(obj, p);
        var isGenericType = p.PropertyType.IsGenericType
                            && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                            && p.PropertyType.GetGenericArguments()[0].IsPrimitive;
        var isNotPrimitive = p.PropertyType == typeof(string)
                             || p.PropertyType == typeof(DateTime)
                             || p.PropertyType == typeof(decimal);

        if (p.PropertyType.IsPrimitive || isNotPrimitive || isGenericType)
        {
            var name = GetPropertyName(p);
            return new[] { Expression.Invoke(lambda, Expression.Constant(name), Expression.Convert(prop, typeof(object))) };
        }

        return GetExpressions(p.PropertyType, prop, lambda);
    }
}