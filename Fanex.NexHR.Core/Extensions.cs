namespace Fanex.NexHR.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.WebPages;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    public static class Extensions
    {
        private const string InputTagBuilder = "input";
        private const string ClassHtmlAttribute = "class";
        private const string ValueHtmlAttribute = "value";
        private const string CheckedHtmlAttribute = "checked";
        private const string CheckboxCustomHtmlClass = "checkbox-custom";
        private static readonly object _O = new object();

        public static MvcHtmlString RadioButtonList<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> listOfValues)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var sb = new StringBuilder();

            if (listOfValues != null)
            {
                // Create a radio button for each item in the list
                foreach (SelectListItem item in listOfValues)
                {
                    // Generate an id to be given to the radio button field
                    var id = string.Format("{0}_{1}", metaData.PropertyName, item.Value);

                    // Create and populate a radio button using the existing html helpers
                    var label = htmlHelper.Label(id, HttpUtility.HtmlEncode(item.Text));
                    var radio = htmlHelper.RadioButtonFor(expression, item.Value, new { id }).ToHtmlString();

                    // Create the html string that will be returned to the client
                    // e.g. <input data-val="true" data-val-required="You must select an option" id="TestRadio_1" name="TestRadio" type="radio" value="1" />
                    // <label for="TestRadio_1">Line1</label>
                    sb.AppendFormat("<div class=\"radioButton\">{0}{1}</div>", radio, label);
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcForm BeginGetForm(this HtmlHelper htmlHelper)
        {
            return htmlHelper.BeginForm(null, null, FormMethod.Get);
        }

        public static DateTime GetMaxDate(this DateTime date)
        {
            if (date.AddMonths(1).Year > DateTimeOffset.Now.Year)
            {
                return DateTimeOffset.Now.AddYears(1).DateTime;
            }

            return date;
        }

        public static MvcHtmlString YearPicker(
            this HtmlHelper htmlHelper,
            string name,
            int fromYear = 2000,
            int currentYear = 2000,
            bool isShowAll = false)
        {
            var list = new List<SelectListItem>();
            var toYear = DateTimeOffset.Now.DateTime.GetMaxDate().Year;

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

            var dropDownList = htmlHelper.DropDownList(name, list, new { @class = "yearpicker form-control" });
            return MvcHtmlString.Create(dropDownList.ToString());
        }

        public static MvcHtmlString YearPickerFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression,
            int fromYear = 2000,
            bool isShowAll = false)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var currentYear = (int)metaData.Model;

            return html.YearPicker(metaData.PropertyName, fromYear, currentYear, isShowAll);
        }

        public static HelperResult RenderTeam(
            this WebPageBase page,
            string teamName,
            Func<object, HelperResult> defaultContent)
        {
            if (page.IsSectionDefined(teamName))
            {
                return page.RenderSection(teamName);
            }

            return defaultContent(_O);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name)
        {
            return CheckBoxList(htmlHelper, name, null);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList)
        {
            return CheckBoxList(htmlHelper, name, selectList, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            return CheckBoxList(htmlHelper, name, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBoxList(
            this HtmlHelper htmlHelper,
            string name,
            IEnumerable<SelectListItem> selectList,
            IDictionary<string, object> htmlAttributes)
        {
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            if (string.IsNullOrEmpty(fullHtmlFieldName))
            {
                throw new ArgumentException("name cannot be null or empty.", nameof(name));
            }

            if (selectList == null)
            {
                throw new ArgumentNullException(nameof(selectList));
            }

            object values = null;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out ModelState state)
               && (state.Value != null))
            {
                values = state.Value.ConvertTo(typeof(string[]), null);
            }

            values = values ?? htmlHelper.ViewData.Eval(fullHtmlFieldName);

            StringBuilder sb = RenderCheckBoxList(name, ref selectList, ref htmlAttributes, fullHtmlFieldName, values);

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return CheckBoxListFor(htmlHelper, expression, null);
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList)
        {
            return CheckBoxListFor(htmlHelper, expression, selectList, (IDictionary<string, object>)null);
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            object htmlAttributes)
        {
            return CheckBoxListFor(htmlHelper, expression, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            IDictionary<string, object> htmlAttributes)
        {
            return CheckBoxList(htmlHelper, ExpressionHelper.GetExpressionText(expression), selectList, htmlAttributes);
        }

        public static MvcHtmlString AttendanceNoteHtml(string notes)
        {
            var htmlString = new StringBuilder();
            if (!string.IsNullOrEmpty(notes))
            {
                string[] noteArray = notes.Split(',');
                for (int i = 0; i < noteArray.Count(); i++)
                {
                    if (noteArray[i] == "No Data" || noteArray[i] == "PH" || noteArray[i].Contains("Off day"))
                    {
                        htmlString.Append(noteArray[i]);
                    }
                    else
                    {
                        bool isLastElement = i + 1 == noteArray.Count();
                        SetLinkValue(noteArray[i], ref htmlString, isLastElement);
                    }
                }
            }
            return MvcHtmlString.Create(htmlString.ToString());
        }

        #region checkbox

        public static MvcHtmlString CheckBoxCustom(this HtmlHelper htmlHelper, string name)
        {
            return CheckBoxCustom(htmlHelper, name, false);
        }

        public static MvcHtmlString CheckBoxCustom(
            this HtmlHelper htmlHelper,
            string name,
            bool isChecked)
        {
            BasicCheckBox(htmlHelper, name, isChecked, out string fullHtmlFieldName, out TagBuilder checkbox);
            return ApplyTagLabel(fullHtmlFieldName, checkbox);
        }

        public static MvcHtmlString CheckBoxCustom(
            this HtmlHelper htmlHelper,
            string name,
            object htmlAttributes)
        {
            return CheckBoxCustom(htmlHelper, name, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBoxCustom(
            this HtmlHelper htmlHelper,
            string name,
            IDictionary<string, object> htmlAttributes)
        {
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            if (string.IsNullOrEmpty(fullHtmlFieldName))
            {
                throw new ArgumentException(Labels.NameCannotBeNullOrEmpty, nameof(name));
            }

            var checkbox = new TagBuilder(InputTagBuilder);
            checkbox.MergeAttribute("id", fullHtmlFieldName);
            checkbox.MergeAttribute("name", fullHtmlFieldName);
            checkbox.MergeAttribute(ClassHtmlAttribute, CheckboxCustomHtmlClass);
            checkbox.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.CheckBox));
            checkbox.MergeAttribute(ValueHtmlAttribute, "true");

            IDictionary<string, object> attributes = (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes);
            checkbox.MergeAttributes(attributes);

            return ApplyTagLabel(fullHtmlFieldName, checkbox);
        }

        public static MvcHtmlString CheckBoxCustom(
            this HtmlHelper htmlHelper,
            string name,
            bool isChecked,
            object htmlAttributes)
        {
            return CheckBoxCustom(htmlHelper, name, isChecked, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBoxCustom(
            this HtmlHelper htmlHelper,
            string name,
            bool isChecked,
            IDictionary<string, object> htmlAttributes)
        {
            BasicCheckBox(htmlHelper, name, isChecked, out string fullHtmlFieldName, out TagBuilder checkbox);

            var attributes = (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes);
            checkbox.MergeAttributes(attributes);

            return ApplyTagLabel(fullHtmlFieldName, checkbox);
        }

        public static MvcHtmlString CheckBoxCustomFor<TModel>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression)
        {
            return CheckBoxCustomFor(htmlHelper, expression, null);
        }

        public static MvcHtmlString CheckBoxCustomFor<TModel>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression,
            object htmlAttributes)
        {
            return CheckBoxCustomFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBoxCustomFor<TModel>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression,
            IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata modelMetadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var propertyNames = expression.Body.ToString().Split('.').ToList();
            var propertyName = propertyNames[propertyNames.Count - 1];

            var checkbox = new TagBuilder(InputTagBuilder);
            checkbox.MergeAttribute("id", propertyName);
            checkbox.MergeAttribute("name", propertyName);
            checkbox.MergeAttribute(ClassHtmlAttribute, CheckboxCustomHtmlClass);
            checkbox.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.CheckBox));
            checkbox.MergeAttribute(ValueHtmlAttribute, "true");
            if (modelMetadata.Model != null && (bool)modelMetadata.Model)
            {
                checkbox.MergeAttribute(CheckedHtmlAttribute, value: CheckedHtmlAttribute);
            }

            var attributes = (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes);
            checkbox.MergeAttributes(attributes);

            var label = new TagBuilder("label");
            label.MergeAttribute("for", propertyName);

            var hidden = new TagBuilder(InputTagBuilder);
            hidden.MergeAttribute("name", propertyName);
            hidden.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Hidden));
            hidden.MergeAttribute(ValueHtmlAttribute, "false");

            return MvcHtmlString.Create(checkbox.ToString(TagRenderMode.SelfClosing) + label + hidden.ToString(TagRenderMode.SelfClosing));
        }

        private static MvcHtmlString ApplyTagLabel(string fullHtmlFieldName, TagBuilder checkbox)
        {
            var label = new TagBuilder("label");
            label.MergeAttribute("for", fullHtmlFieldName);

            var hidden = new TagBuilder(InputTagBuilder);
            hidden.MergeAttribute("name", fullHtmlFieldName);
            hidden.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Hidden));
            hidden.MergeAttribute(ValueHtmlAttribute, "false");

            return MvcHtmlString.Create(checkbox.ToString(TagRenderMode.SelfClosing) + label + hidden.ToString(TagRenderMode.SelfClosing));
        }

        private static void BasicCheckBox(HtmlHelper htmlHelper, string name, bool isChecked, out string fullHtmlFieldName, out TagBuilder checkbox)
        {
            fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (string.IsNullOrEmpty(fullHtmlFieldName))
            {
                throw new ArgumentException(Labels.NameCannotBeNullOrEmpty, nameof(name));
            }

            checkbox = new TagBuilder(InputTagBuilder);
            checkbox.MergeAttribute("id", fullHtmlFieldName);
            checkbox.MergeAttribute("name", fullHtmlFieldName);
            checkbox.MergeAttribute(ClassHtmlAttribute, CheckboxCustomHtmlClass);
            checkbox.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.CheckBox));
            checkbox.MergeAttribute(ValueHtmlAttribute, "true");
            if (isChecked)
            {
                checkbox.MergeAttribute(CheckedHtmlAttribute, value: CheckedHtmlAttribute);
            }
        }

        #endregion checkbox

        private static void SetLinkValue(string noteValue, ref StringBuilder htmlString, bool isLastElement)
        {
            var tag = new TagBuilder("a");
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

            tag.MergeAttribute("href", link);
            tag.SetInnerText(noteValue);
            htmlString.Append(tag.ToString(TagRenderMode.Normal));

            if (!isLastElement)
            {
                htmlString.Append(",");
            }
        }

        #region Business

        public static string RequestTypeToText(this short type)
        {
            switch (type)
            {
                case (byte)RequestType.CheckIn:
                    return Labels.CheckIn;

                case (byte)RequestType.CheckOut:
                    return Labels.CheckOut;

                case (byte)RequestType.LateIn:
                    return Labels.LateIn;

                case (byte)RequestType.EarlyOut:
                    return Labels.EarlyOut;

                case (byte)RequestType.ChangeShift:
                    return Labels.ChangeShift;

                case (byte)RequestType.Leave:
                    return Labels.Leave;

                case (byte)RequestType.KeepInView:
                    return Labels.CompensationLeave;

                case (byte)RequestType.Overtime:
                    return Labels.Overtime;

                default:
                    return string.Empty;
            }
        }

        public static MvcHtmlString DropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            SelectListItem firstItem,
            object htmlAttributes)
        {
            IEnumerable<SelectListItem> outputSelectList = null;

            if (firstItem != null && firstItem.Value != null)
            {
                var list = new List<SelectListItem>
                {
                    firstItem
                };

                if (selectList != null)
                {
                    foreach (SelectListItem item in selectList)
                    {
                        list.Add(item);
                    }
                }

                outputSelectList = list;
            }

            return SelectExtensions.DropDownListFor(htmlHelper, expression, outputSelectList, htmlAttributes);
        }

        public static MvcHtmlString EnumDropDownFor<TModel, TEnum>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression,
            Func<TEnum, bool> predicate = null, string optionLabel = null,
            object htmlAttributes = null) where TEnum : struct, IConvertible
        {
            var enumList = predicate == null ? Enum.GetValues(typeof(TEnum)).Cast<TEnum>()
                : Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Where(e => predicate(e));

            var selectList = enumList
                .Select(e => new SelectListItem
                {
                    Value = Convert.ToUInt64(e).ToString(),
                    Text = Helpers.EnumHelper.GetStringValue((Enum)(object)e),
                }).ToList();
            if (!string.IsNullOrEmpty(optionLabel))
            {
                selectList.Insert(0, new SelectListItem
                {
                    Text = optionLabel
                });
            }

            return htmlAttributes == null
                ? htmlHelper.DropDownListFor(expression, selectList)
                : htmlHelper.DropDownListFor(expression, selectList, htmlAttributes);
        }

        #endregion Business

        private static StringBuilder RenderCheckBoxList(
            string name,
            ref IEnumerable<SelectListItem> selectList,
            ref IDictionary<string, object> htmlAttributes,
            string fullHtmlFieldName,
            object values)
        {
            if (values != null)
            {
                var valueList = values.GetType() == typeof(string[]) ? values as string[] : values.ToString().Split(',');

                var list = new List<SelectListItem>();
                foreach (SelectListItem item in selectList)
                {
                    item.Selected = (item.Value != null) && valueList.Contains(item.Value);
                    list.Add(item);
                }

                selectList = list;
            }

            var sb = new StringBuilder();
            htmlAttributes = (htmlAttributes == null) ? new RouteValueDictionary() : new RouteValueDictionary(htmlAttributes);
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
                    input.MergeAttribute(CheckedHtmlAttribute, value: CheckedHtmlAttribute);
                }

                var label = new TagBuilder("label")
                {
                    InnerHtml = item.Text
                };
                label.MergeAttribute("for", name + item.Value);

                var div = new TagBuilder("div")
                {
                    InnerHtml = input.ToString(TagRenderMode.SelfClosing) + label
                };
                div.MergeAttribute(ClassHtmlAttribute, "checkbox");
                sb.Append(div.ToString());
            }

            return sb;
        }
    }
}