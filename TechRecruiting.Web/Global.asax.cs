using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TechRecruiting.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            ConfigureUnity();

            void RegisterGlobalFilters(GlobalFilterCollection filters)
            {
                filters.Add(new HandleErrorAttribute());
            }

            void RegisterRoutes(RouteCollection routes)
            {
                routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

                routes.MapMvcAttributeRoutes();
            }

            void ConfigureUnity()
            {
                IUnityContainer container = new UnityContainer();
                DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            }
        }
    }
}