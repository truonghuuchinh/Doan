using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Areas.Administration.Controllers
{
    public class UserController : BaseController
    {
        [Area("Administration")]
        public IActionResult Index()
        {
            @ViewBag.TitlePage = "Thông tin tài khoản";
            return View();
        }
        public IActionResult InforUser()
        {
            return View();
        }
    }
}
