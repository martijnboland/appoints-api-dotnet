using System.Web.Http;
using WebApi.Hal;

namespace Appoints.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new {controller = "Index", id = RouteParameter.Optional}
                );

            // Formatters
            config.Formatters.Insert(0, new JsonHalMediaTypeFormatter());
            config.Formatters.Add(new XmlHalMediaTypeFormatter());
        }
    }
}