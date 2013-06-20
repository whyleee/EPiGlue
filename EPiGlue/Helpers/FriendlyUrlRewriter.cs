using System.Web;
using System.Web.Routing;
using EPiServer;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Perks;

namespace EPiGlue.Helpers
{
    public class FriendlyUrlRewriter
    {
        public virtual string GetFriendlyUrl(string internalUrl, RequestContext requestContext, RouteCollection routeCollection)
        {
            if (internalUrl.IsNullOrEmpty() || internalUrl.StartsWith("#"))
            {
                return internalUrl;
            }

            return MapUrlFromRoute(internalUrl, requestContext, routeCollection);
        }

        // Decompiled
        internal static string MapUrlFromRoute(string internalUrl, RequestContext requestContext, RouteCollection routeCollection)
        {
            internalUrl = GetMappedHref(internalUrl);
            var mappedUrl = new UrlBuilder(HttpUtility.HtmlDecode(internalUrl));
            var contentReference = PermanentLinkUtility.GetContentReference(mappedUrl);

            if (ContentReference.IsNullOrEmpty(contentReference))
            {
                return mappedUrl.ToString();
            }

            var language = mappedUrl.QueryCollection["epslanguage"] ?? requestContext.GetLanguage() ?? ContentLanguage.PreferredCulture.Name;
            var setIdAsQueryParameter = false;
            bool result;

            if (!string.IsNullOrEmpty(requestContext.HttpContext.Request.QueryString["idkeep"]) &&
                !bool.TryParse(requestContext.HttpContext.Request.QueryString["idkeep"], out result))
            {
                setIdAsQueryParameter = result;
            }

            return routeCollection.GetVirtualPath(contentReference, language, setIdAsQueryParameter, false).GetUrl();
        }

        public static string GetMappedHref(string linkItemUrl)
        {
            string mappedUrl;
            var permanentLinkMapper = ServiceLocator.Current.GetInstance<IPermanentLinkMapper>();

            if (!permanentLinkMapper.TryToMapped(linkItemUrl, out mappedUrl))
            {
                return linkItemUrl;
            }

            return (string) new UrlBuilder(mappedUrl);
        }
    }
}