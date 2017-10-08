using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeFee.WebApplication.Infrastructure.Extensions
{
    public class RouteInfo
    {
        public RouteInfo(string area, string controller, string action)
        {
            this.Action = action;
            this.Area = area;
            this.Controller = controller;
        }

        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        public bool Is(string area, string controller, string action)
        {
            return this.Area.Equals(area, StringComparison.InvariantCultureIgnoreCase) && Is(controller, action);
        }

        public bool Is(string controller, string action)
        {
            return this.Action.Equals(action, StringComparison.InvariantCultureIgnoreCase) &&
                   this.Controller.Equals(controller, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
