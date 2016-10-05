﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AAAService.Helpers;

namespace AAAService
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // This will add the parameter "subdomain" to the route parameters
            routes.Add(new SubdomainRoute());

            //routes.MapRoute(
            //    name: "ServiceTicket",
            //    url: "service_tickets/{action}/{parameter1}",
            //    defaults: new { controller = "service_tickets", action = "Create", id = UrlParameter.Optional });

            //routes.MapRoute(
            //    name: "Home2",
            //    url: "",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);           

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}