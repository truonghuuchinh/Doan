using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Areas.Administration.Controllers
{
    public class HomeController : Controller
    {
        [Area("Administration")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
