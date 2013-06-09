using System.Web;

namespace EPiGlue
{
    public interface IEditHtmlString : IHtmlString
    {
        IHtmlString EditorStart { get; set; }

        IHtmlString EditorEnd { get; set; }

        IHtmlString DefaultValue { get; set; }
    }
}