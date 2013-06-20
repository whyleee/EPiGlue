using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using EPiGlue.DataAnnotations;
using EPiGlue.Framework;
using EPiServer.Editor;
using EPiServer.Framework.Serialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using Perks;

namespace EPiGlue.Handlers
{
    public class PageEditorHtmlHandler : PageEditorFieldHandler
    {
        public override void Process(ModelPropertyContext context)
        {
            base.Process(context);

            var fieldHtml = InsertPageEditorMarkup(context, (IHtmlString) context.PropertyValue);
            context.PropertyValue = fieldHtml; // TODO: we could have an exception here (different types of property and setting value)
        }

        private IHtmlString InsertPageEditorMarkup(ModelPropertyContext context, IHtmlString fieldHtml)
        {
            var editHtml = fieldHtml is IEditHtmlString ? (IEditHtmlString) fieldHtml : new EditableHtmlString(fieldHtml);
            var htmlBuilder = new StringBuilder();
            var editHint = context.Property.Get<EditHintAttribute>() ?? new EditHintAttribute();

            using (var writer = new StringWriter(htmlBuilder))
            {
                using (var editContainer = new MvcEditContainer(
                    requestContext: context.ExecutionContext.RequestContext,
                    epiPropertyKey: PageEditing.DataEPiPropertyName,
                    epiPropertyName: GetFieldEditName(context),
                    editElementName: editHint.EditTag,
                    editElementCssClass: editHint.EditCssClass,
                    writer: writer))
                {
                    Func<string> renderSettingsWriter = null;
                    var renderSettings = new RouteValueDictionary();
                    
                    if (editHint.CssClass.IsNotNullOrEmpty())
                    {
                        renderSettings.Add("CssClass", editHint.CssClass);
                    }
                    if (editHint.ChildrenCssClass.IsNotNullOrEmpty())
                    {
                        renderSettings.Add("ChildrenCssClass", editHint.ChildrenCssClass);
                    }

                    renderSettingsWriter = () => AttrsWriter(renderSettings, PageEditing.DataEPiPropertyRenderSettings);

                    editContainer.CreateStartElementForEditMode(renderSettingsWriter);
                    editHtml.EditorStart = new HtmlString(writer.ToString());
                    htmlBuilder.Clear();
                }

                editHtml.EditorEnd = new HtmlString(writer.ToString());
            }

            return editHtml;
        }

        private static string AttrsWriter(RouteValueDictionary routeValues, string attributeName)
        {
            // TODO: decompiled and could be much easier I think
            if (routeValues.Count == 0)
            {
                return null;
            }
            var instance = ServiceLocator.Current.GetInstance<IObjectSerializerFactory>();
            var serializer = instance.GetSerializer("application/json");
            var stringBuilder = new StringBuilder();
            serializer.Serialize(new StringWriter(stringBuilder), routeValues);
            var cssClass = routeValues["cssclass"] as string;
            return string.Format("{0}=\"{1}\"{2}", attributeName, HttpUtility.HtmlAttributeEncode(stringBuilder.ToString()),
                string.IsNullOrEmpty(cssClass) ? string.Empty : string.Format(CultureInfo.InvariantCulture, " class=\"{0}\"", new object[]{cssClass}));
        }
    }
}
