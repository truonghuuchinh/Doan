using DoanApp.Areas.Administration.Models;
using DoanApp.Commons;
using DoanApp.ServiceApi;
using DoanApp.Services;
using DoanData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DoanApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _usermanager;
        private readonly IFollowChannelService _followChanel;
        private readonly IVideoService _videoService;
        private readonly IReportVideoService _reportService;
        private readonly ICategoryService _categoryService;
        private readonly IUserApiCient _userApiClient;
        public UserController(IUserService userService,UserManager<AppUser> user,IFollowChannelService follow,
            IVideoService video,IReportVideoService report,ICategoryService category,
            IUserApiCient userApi)
        {
            _userService = userService;
            _usermanager = user;
            _followChanel = follow;
            _videoService = video;
            _reportService = report;
            _categoryService = category;
            _userApiClient = userApi;
        }
        public IActionResult AnalysisUser(int id)
        {
            ViewBag.IdUser = id;
            ViewBag.UserFollow = _followChanel.GetAll().Where(x => x.ToUserId == id).Count();
            ViewBag.CountViewChannel = _videoService.GetAll().Where(x => x.AppUserId == id).Sum(x => x.ViewCount);
            ViewBag.CountVideo = _videoService.GetAll().Where(x=>x.AppUserId==id).Count();
            ViewBag.CountReport = (from report in _reportService.GetAll()
                                   join video in _videoService.GetAll().Where(x=>x.AppUserId==id) on report.VideoId equals video.Id
                                   select report).Count();
            ViewBag.CoutCategory = _videoService.GetAll().Where(x=>x.AppUserId==id).GroupBy(x => x.CategorysId).Count();
            var listRankedVideo = _videoService.GetAll().Where(x => x.AppUserId == id).OrderByDescending(x => x.ViewCount).Take(10).ToList();
            return View(listRankedVideo);
        }
        public string GetCreateDate(int id)
        {
            if (id != 0)
            {
                var listVideo = _videoService.GetAll().Where(x => x.AppUserId == id).
                    GroupBy(x => x.CreateDate.Split('-')[0]).Select(x => new
                    {
                        Month=int.Parse(x.Key),
                        Count=x.Count()
                    }).OrderBy(x=>x.Month).ToList();
                return JsonConvert.SerializeObject(listVideo);
            }
            return null;
        }
        public string GetCategory(int idUser)
        {
            if (idUser != 0)
            {
                var list = _videoService.GetAll().Where(x => x.AppUserId == idUser).
                    GroupBy(x => x.CategorysId).Select(x => new
                    {
                        Id=x.Key,
                        Count=x.Count()
                    });
                var listCategory=(from video in list
                                 join cate in _categoryService.GetAll().Result on video.Id equals cate.Id
                                 select new
                                 {
                                     cate.Name,
                                     video.Count
                                 }).OrderByDescending(x=>x.Count).ToList();
                return JsonConvert.SerializeObject(listCategory);
            }
            return null;
        }
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;

            ViewBag.Active = 1;
            var Token = HttpContext.Session.GetString("Token");
            if (_userApiClient.CheckToken(Token, User.Identity.Name) == null)
                return Redirect("/Administration/Home/Login");
            else
            {
                if (Token == null)
                    Token = await _userApiClient.CheckToken(Token, User.Identity.Name);
            }
            var listUserAdmin = await _userApiClient.GetAllUserAdmin(Token, User.Identity.Name);
            foreach (var item in listUserAdmin)
            {
                var us = new AppUser();
            }

            return View(listUserAdmin.ToPagedList(pageNumber, pageSize));
        }
        public IActionResult Index_Partial(int? page, string name)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;
            var listUserAdmin = _userService.GetUserAdmin(User.Identity.Name);
            if (name != null)
            {
                name = ConvertUnSigned.convertToUnSign(name).ToLower();
                listUserAdmin = listUserAdmin.Where(x => ConvertUnSigned.convertToUnSign(x.Name).
                ToLower().Contains(name)).ToList();
            }
            var list = listUserAdmin.ToPagedList(pageNumber, pageSize);
            return View(list);
        }
        public ActionResult InforUser()
        {
            var user = UserAuthenticated.GetUser(User.Identity.Name);
           
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id != 0)
            {
                var Token = HttpContext.Session.GetString("Token");
                if (_userApiClient.CheckToken(Token, User.Identity.Name) == null)
                    return Redirect("/Administration/Home/Login");
                else
                {
                    if (Token == null)
                        Token = await _userApiClient.CheckToken(Token, User.Identity.Name);
                }
                var result = await _userApiClient.Delete(Token,id);
                if (result > 0) return Content("Success");

            }
            return Content("Error");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLock(int id)
        {
            if (id != 0)
            {
                var user = await _userService.FindUserId(id);
                if (user != null)
                {
                    var result = await _userService.UpdateLockout(user);
                    if (result > 0) return Content("Success");
                }
            }
            return Content("Error");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAvartar(IFormFile fileupload,string emailUser)
        {

            if (emailUser != null && fileupload != null)
            {
                var user = await _userService.FindUser(emailUser);
                if (user.Avartar != null && user.LoginExternal == false)
                    System.IO.File.Delete("wwwroot/Client/avartar/" + user.Avartar);
                var filename = fileupload.FileName.Split('.');
                var name = user.Id.ToString() + "." + filename[filename.Length - 1].ToLower();
                user.Avartar = name;
                using (var fileStream = new FileStream(Path.Combine("wwwroot/Client/avartar", name),
                            FileMode.Create, FileAccess.Write))
                {
                    fileupload.CopyTo(fileStream);
                }
                var result = await _userService.UpdateAvartar(user.Id, name);
                if (result > 0)
                {
                    new UserAuthenticated().UpdateAvartar(user.Id, name);
                    return Redirect("InforUser");
                }
            }
            return Redirect("InforUser");
        }
        public IActionResult TranferAccount()
        {
            return View();
        }
    }
}
