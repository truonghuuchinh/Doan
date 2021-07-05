using DoanApp.Services;
using DoanData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Areas.Administration.Controllers
{
    public class BaseController : Controller
    {
       
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin")|| User.IsInRole("Manager"))
                {
                   
                }else
                {
                    context.Result = new RedirectToRouteResult(new
                       RouteValueDictionary(new { controller = "Home", action = "Login", area = "Administration" }));
                }
            }
            else
            {
                context.Result = new RedirectToRouteResult(new
                       RouteValueDictionary(new { controller = "Home", action = "Login", area = "Administration" }));
            }
            base.OnActionExecuting(context);
        }
    }
}
