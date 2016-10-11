using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AAAService.Helpers
{
    public class SubdomainRoute : RouteBase
    {
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            if (httpContext.Request == null || httpContext.Request.Url == null)
            {
                return null;
            }

            var host = httpContext.Request.Url.Host;
            var index = host.IndexOf(".");
            string[] segments = httpContext.Request.Url.PathAndQuery.TrimStart('/').Split('/');

            if (index < 0)
            {
                return null;
            }

            var subdomain = host.Substring(0, index);
            string[] blacklist = { "www", "assetsaaa", "mail" };

            if (blacklist.Contains(subdomain))
            {
                return null;
            }

            string controller = "Home", action = "Index";
            Guid id = Guid.Empty;

            if (segments.Length > 0 && segments[0] != "")
            {
                controller = (segments.Length > 0) ? segments[0] : "Home";
                action = (segments.Length > 1) ? segments[1] : "Index";
                id = Guid.Parse((segments.Length > 2 && segments[2] != "") ? segments[2] : Guid.Empty.ToString());
            }

            var routeData = new RouteData(this, new MvcRouteHandler());
            routeData.Values.Add("controller", controller); //Goes to the relevant Controller  class
            routeData.Values.Add("action", action); //Goes to the relevant action method on the specified Controller
            routeData.Values.Add("subdomain", subdomain); //pass subdomain as argument to action method
            //routeData.Values.Add("id", UrlParameter.Optional);
            routeData.Values.Add("id", id); //pass id as argument to action method
            return routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            //Implement your formating Url formating here
            return null;
        }
    }
}