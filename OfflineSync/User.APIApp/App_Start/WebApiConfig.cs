using System.Web.Http;

namespace User.APIApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Web API routes
            config.MapHttpAttributeRoutes();

            // Registering Config Route
            OfflineSync.Server.Config.Register(config);

            OfflineSync.Server.DB.SQLServerDBUtility.CreateGlobalTrigger();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}