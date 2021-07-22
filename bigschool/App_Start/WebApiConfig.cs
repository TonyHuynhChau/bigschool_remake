using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Http;

namespace BigSchool.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            //configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{id}",
            //  new { id = RouteParameter.Optional });

            configuration.MapHttpAttributeRoutes();

            configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}