using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SUVAN.BackOffice.API
{
    public static class MvcOptionsExtensions
    {
        public static  void UseGeneralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            opts.Conventions.Add(new RoutePrefixConventions(routeAttribute));

        }
        public static void GeneralRoutePrefix(this MvcOptions opts,string prefix)
        {
            opts.UseGeneralRoutePrefix(new RouteAttribute(prefix));
        }
    }
    public class RoutePrefixConventions : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _routePrefix;
        public RoutePrefixConventions(IRouteTemplateProvider route) {
            _routePrefix = new AttributeRouteModel(route);
        }

        public void Apply(ApplicationModel application)
        {
            foreach(SelectorModel item in application.Controllers.SelectMany((ControllerModel c) => c.Selectors)){
                if(item.AttributeRouteModel != null)
                {
                    item.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, item.AttributeRouteModel);
                }
                else
                {
                    item.AttributeRouteModel = _routePrefix;
                }
                
            }
            
        }
    }
}
