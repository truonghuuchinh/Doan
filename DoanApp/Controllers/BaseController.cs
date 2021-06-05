using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(new
                       RouteValueDictionary(new { controller = "Home", action = "Login"}));
            }
        }
    }
}
