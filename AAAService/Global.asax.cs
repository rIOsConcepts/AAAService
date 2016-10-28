using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace AAAService
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //if (HttpContext.Current.Request.ServerVariables["server_name"] == "assetsaaa.com")
            //{
            //    if (int.Parse(HttpContext.Current.Request.ServerVariables["server_port"]) == 443)
            //    {
            //        Response.Redirect("http://assetsaaa.com/");
            //    }
            //}
            //else
            //{
            //    if (int.Parse(HttpContext.Current.Request.ServerVariables["server_port"]) != 443)
            //    {
            //        Response.Redirect("https://rabobank.assetsaaa.com/");
            //    }
            //}
        }

        protected void Application_BeginRequest()
        {
            if (Context.Request.Url.ToString().ToLower().Contains("rabobank"))
            {
                if (!Context.Request.IsSecureConnection)
                {
                    Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
                }
            }
            else
            {
                if (Context.Request.IsSecureConnection)
                {
                    Response.Redirect(Context.Request.Url.ToString().Replace("https:", "http:"));
                }
            }
        }
    }
}