using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace OrchardHUN.ModuleProfiles
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes()) routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor
                {
                    Priority = 2,
                    Route = new Route(
                        "Modules/ModuleProfiles/{profileName}",
                        new RouteValueDictionary
                        {
                            {"area", "OrchardHUN.ModuleProfiles"},
                            {"controller", "ModuleProfilesAdmin"},
                            {"action", "Index"},
                        },
                        new RouteValueDictionary (),
                        new RouteValueDictionary
                        {
                            {"area", "OrchardHUN.ModuleProfiles"}
                        },
                        new MvcRouteHandler()
                    )
                },
                new RouteDescriptor
                {
                    Priority = 1,
                    Route = new Route(
                        "Modules/ModuleProfiles",
                        new RouteValueDictionary
                        {
                            {"area", "OrchardHUN.ModuleProfiles"},
                            {"controller", "ModuleProfilesAdmin"},
                            {"action", "Index"},
                        },
                        new RouteValueDictionary (),
                        new RouteValueDictionary
                        {
                            {"area", "OrchardHUN.ModuleProfiles"}
                        },
                        new MvcRouteHandler()
                    )
                }
            };
        }
    }
}