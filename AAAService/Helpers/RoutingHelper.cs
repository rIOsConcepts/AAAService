using System;
using System.Linq;
using System.Text.RegularExpressions;
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

            string controller = "Home", action = "Index", userId = "", code = "";
            Guid id = Guid.Empty;

            if (segments.Length > 0 && segments[0] != "")
            {
                controller = (segments.Length > 0) ? segments[0] : "Home";
                action = (segments.Length > 1) ? segments[1] : "Index";

                if (action.Contains("?") && action.Contains("&"))
                {
                    userId = action.Substring(action.IndexOf("?userId=") + 8, action.IndexOf("&code=") - action.IndexOf("?userId=") - 8);
                    code = action.Substring(action.IndexOf("&code=") + 6, action.Length - action.IndexOf("&code=") - 6);
                    code = code.Replace("%2B", "+").Replace("%2F", "/").Replace("%3D", "=");
                    action = action.Substring(0, action.IndexOf("?userId="));
                }
                else
                {
                    id = Guid.Parse((segments.Length > 2 && segments[2] != "") ? segments[2] : Guid.Empty.ToString());
                }
            }

            var routeData = new RouteData(this, new MvcRouteHandler());
            routeData.Values.Add("controller", controller); //Goes to the relevant Controller  class
            routeData.Values.Add("action", action); //Goes to the relevant action method on the specified Controller
            routeData.Values.Add("subdomain", subdomain); //pass subdomain as argument to action method
            //routeData.Values.Add("id", UrlParameter.Optional);

            if (userId != "" && code != "")
            {
                routeData.Values.Add("userId", userId); //pass userId as argument to action method
                routeData.Values.Add("code", code); //pass code as argument to action method
            }
            else
            {
                routeData.Values.Add("id", id); //pass id as argument to action method
            }

            return routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            //Implement your formating Url formating here
            return null;
        }
    }
}