using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class UserController : BaseController
    {
        public IActionResult Index()
        {
            @ViewBag.TitlePage = "Quản lý Người dùng";
            return View();
        }
       public ActionResult InforUser()
        {
            @ViewBag.TitlePage = "Thông tin tài khoản";
            return View();
        }
    }
}
