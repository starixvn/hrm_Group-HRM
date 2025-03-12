namespace HRM.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using Enums;
using Resources;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

public static class Extensions
{
    private const string InputTagBuilder = "input";
    private const string ClassHtmlAttribute = "class";
    private const string ValueHtmlAttribute = "value";
    private const string CheckedHtmlAttribute = "checked";
    private const string CheckboxCustomHtmlClass = "checkbox-custom";
    public static string RequestTypeToText(this short type)
    {
        return type switch
        {
            (byte)RequestType.CheckIn => Labels.CheckIn,
            (byte)RequestType.CheckOut => Labels.CheckOut,
            (byte)RequestType.LateIn => Labels.LateIn,
            (byte)RequestType.EarlyOut => Labels.EarlyOut,
            (byte)RequestType.ChangeShift => Labels.ChangeShift,
            (byte)RequestType.Leave => Labels.Leave,
            (byte)RequestType.KeepInView => Labels.CompensationLeave,
            (byte)RequestType.Overtime => Labels.Overtime,
            _ => string.Empty
        };
    }

    public static List<SelectListItem> YearPicker(
        int fromYear = 2000,
        int currentYear = 2000,
        bool isShowAll = false)
    {
        var list = new List<SelectListItem>();
        var toYear = DateTimeOffset.Now.DateTime.Year;

        if (isShowAll)
        {
            list.Add(new SelectListItem
            {
                Text = Labels.All,
                Value = "0"
            });
        }

        for (var i = toYear + 1; i >= fromYear; i--)
        {
            list.Add(new SelectListItem
            {
                Text = i.ToString(),
                Value = i.ToString(),
                Selected = i == currentYear
            });
        }

        return list;
    }

    public static List<SelectListItem> YearPickerFor(
        int fromYear = 2000,
        bool isShowAll = false)
    {
        var currentYear = DateTimeOffset.Now.DateTime.Year;

        return YearPicker(fromYear, currentYear, isShowAll);
    }

    public static DateTime GetMaxDate(this DateTime date)
    {
        if (date.AddMonths(1).Year > DateTimeOffset.Now.Year)
        {
            return DateTimeOffset.Now.AddYears(1).DateTime;
        }

        return date;
    }

    public static IHtmlContent CheckBoxCustomFor<TModel>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, bool>> expression)
    {
        return CheckBoxCustomFor(htmlHelper, expression, htmlAttributes: null);
    }

    private static IHtmlContent CheckBoxCustomFor<TModel>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, bool>> expression,
        IDictionary<string, object> htmlAttributes)
    {
        var propertyName = htmlHelper.NameFor(expression) ?? string.Empty;

        var modelObject = htmlHelper.ViewData.Model;
        var currentValue = expression.Compile().Invoke(modelObject);

        var checkbox = new TagBuilder("input")
        {
            TagRenderMode = TagRenderMode.SelfClosing,
            Attributes =
            {
                ["type"] = "checkbox",
                ["id"] = propertyName,
                ["name"] = propertyName,
                ["value"] = "true"
            }
        };
        checkbox.AddCssClass("checkbox-custom");

        if (currentValue)
        {
            checkbox.Attributes["checked"] = "checked";
        }

        if (htmlAttributes != null)
        {
            foreach (var kvp in htmlAttributes)
            {
                checkbox.MergeAttribute(kvp.Key, kvp.Value?.ToString(), replaceExisting: true);
            }
        }

        var label = new TagBuilder("label")
        {
            TagRenderMode = TagRenderMode.Normal,
            Attributes =
            {
                ["for"] = propertyName
            }
        };

        var hidden = new TagBuilder("input")
        {
            TagRenderMode = TagRenderMode.SelfClosing,
            Attributes =
            {
                ["type"] = "hidden",
                ["name"] = propertyName,
                ["value"] = "false"
            }
        };

        return new HtmlContentBuilder()
            .AppendHtml(checkbox)
            .AppendHtml(label)
            .AppendHtml(hidden);
    }

    public static IHtmlContent CheckBoxListFor<TModel, TProperty>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TProperty>> expression,
        IEnumerable<SelectListItem> selectList)
    {
        return CheckBoxListFor(htmlHelper, expression, selectList, htmlAttributes: null);
    }

    private static IHtmlContent CheckBoxListFor<TModel, TProperty>(
        this IHtmlHelper<TModel> htmlHelper,
        Expression<Func<TModel, TProperty>> expression,
        IEnumerable<SelectListItem> selectList,
        IDictionary<string, object> htmlAttributes)
    {
        var expressionText = GetExpressionText(expression);
        return CheckBoxList(htmlHelper, expressionText, selectList, htmlAttributes);
    }

    private static IHtmlContent CheckBoxList(
        this IHtmlHelper htmlHelper,
        string name,
        IEnumerable<SelectListItem> selectList,
        IDictionary<string, object> htmlAttributes)
    {
        var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
        if (string.IsNullOrEmpty(fullHtmlFieldName))
        {
            throw new ArgumentException("name cannot be null or empty.", nameof(name));
        }
        if (selectList == null)
        {
            throw new ArgumentNullException(nameof(selectList));
        }

        object values = null;
        if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out var state) &&
            state.RawValue != null)
        {
            values = state.RawValue;
        }

        values ??= htmlHelper.ViewData.Eval(fullHtmlFieldName);

        var sb = RenderCheckBoxList(name, ref selectList, ref htmlAttributes, fullHtmlFieldName, values);

        return new HtmlString(sb.ToString());
    }

    private static string GetExpressionText<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
    {
        var memberExp = RemoveConvert(expression.Body) as MemberExpression;
        if (memberExp == null)
        {
            throw new ArgumentException("The expression is not a valid member expression.", nameof(expression));
        }
        return memberExp.Member.Name;
    }

    private static Expression RemoveConvert(Expression expression)
    {
        while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
        {
            expression = ((UnaryExpression)expression).Operand;
        }
        return expression;
    }

    private static StringBuilder RenderCheckBoxList(
        string name,
        ref IEnumerable<SelectListItem> selectList,
        ref IDictionary<string, object> htmlAttributes,
        string fullHtmlFieldName,
        object values)
    {
        if (values != null)
        {
            var valueList = values.GetType() == typeof(string[])
                ? values as string[]
                : values.ToString()?.Split([','], StringSplitOptions.RemoveEmptyEntries);

            var list = new List<SelectListItem>();
            foreach (var item in selectList)
            {
                if (valueList != null)
                {
                    item.Selected = item.Value != null && valueList.Contains(item.Value); 
                }
                list.Add(item);
            }
            selectList = list;
        }

        var sb = new StringBuilder();
        htmlAttributes = htmlAttributes == null ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes);
        htmlAttributes.Remove(CheckedHtmlAttribute);

        foreach (var item in selectList)
        {
            var input = new TagBuilder(InputTagBuilder);
            input.MergeAttributes(htmlAttributes);
            input.MergeAttribute("type", "checkbox");
            input.MergeAttribute("id", name + item.Value);
            input.MergeAttribute(ClassHtmlAttribute, CheckboxCustomHtmlClass);
            input.MergeAttribute("name", fullHtmlFieldName, replaceExisting: true);
            input.MergeAttribute(ValueHtmlAttribute, item.Value ?? item.Text ?? string.Empty, replaceExisting: true);
            if (item.Selected)
            {
                input.MergeAttribute(CheckedHtmlAttribute, CheckedHtmlAttribute);
            }

            var label = new TagBuilder("label");
            label.MergeAttribute("for", name + item.Value);
            label.InnerHtml.Append(item.Text);

            var div = new TagBuilder("div");
            div.MergeAttribute(ClassHtmlAttribute, "checkbox");
            div.InnerHtml.AppendHtml(input);
            div.InnerHtml.AppendHtml(label);

            sb.Append(div.GetString());
        }

        return sb;
    }

    private static string GetString(this TagBuilder tagBuilder)
    {
        using var writer = new System.IO.StringWriter();
        tagBuilder.WriteTo(writer, HtmlEncoder.Default);
        return writer.ToString();
    }

    public static HtmlString AttendanceNoteHtml(string notes)
    {
        var htmlString = new StringBuilder();

        if (!string.IsNullOrEmpty(notes))
        {
            var noteArray = notes.Split(',');

            for (var i = 0; i < noteArray.Length; i++)
            {
                if (noteArray[i] == "No Data" || noteArray[i] == "PH" || noteArray[i].Contains("Off day"))
                {
                    htmlString.Append(noteArray[i]);
                }
                else
                {
                    var isLastElement = (i + 1 == noteArray.Length);
                    SetLinkValue(noteArray[i], ref htmlString, isLastElement);
                }
            }
        }

        return new HtmlString(htmlString.ToString());
    }

    private static void SetLinkValue(string noteValue, ref StringBuilder htmlString, bool isLastElement)
    {
        string link;

        if (noteValue.Contains("Missing"))
        {
            link = "/checkinout/employeeview";
        }
        else if (noteValue.Contains("Late") || noteValue.Contains("Early"))
        {
            link = "/lateinearlyout/employeeview";
        }
        else if (noteValue.Contains("KIV"))
        {
            link = "/keepinview/employeeview";
        }
        else
        {
            link = "/leave/employeeview";
        }

        var safeNoteValue = HtmlEncoder.Default.Encode(noteValue);

        htmlString.Append($"<a href='{link}'>{safeNoteValue}</a>");

        if (!isLastElement)
        {
            htmlString.Append(", ");
        }
    }
}