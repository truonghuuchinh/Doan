using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
