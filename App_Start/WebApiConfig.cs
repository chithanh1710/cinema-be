using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CINEMA_BE
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Xóa tất cả các bộ định dạng XML
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Thêm bộ định dạng JSON
            config.Formatters.JsonFormatter.SupportedMediaTypes.Clear();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));


            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
