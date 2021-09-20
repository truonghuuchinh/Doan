using DoanApp.Areas.Administration.Models;
using DoanApp.Commons;
using DoanApp.Models;
using DoanApp.ServiceApi;
using DoanApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DoanApp.Areas.Administration.Controllers
{
    [Authorize]
    [Area("Administration")]
    public class ReportVideoController : BaseController
    {
        private readonly IReportVideoService _reportVideoService;
        private readonly IVideoService _videoService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IReportApiClient _reportApiClient;
        private readonly IUserApiCient _userApiClient;
        public ReportVideoController(IReportVideoService reportVideoService,IVideoService videoService,
            IUserService user,INotificationService notification, IReportApiClient reportApiClient,
            IUserApiCient userApiCient)
        {
            _reportVideoService = reportVideoService;
            _videoService = videoService;
            _userService = user;
            _notificationService = notification;
            _reportApiClient = reportApiClient;
            _userApiClient = userApiCient;
        }
        public async Task<IActionResult> Index(int? page)
        {
            if (page == null) page = 1;
            int pageNumber = page ?? 1;
            int pageSize = 6;
            ViewBag.Active = 4;
            var Token = HttpContext.Session.GetString("Token");
            if (_userApiClient.CheckToken(Token, User.Identity.Name) == null)
                return Redirect("/Administration/Home/Login");
            else
            {
                if (Token == null)
                    Token = await _userApiClient.CheckToken(Token, User.Identity.Name);
            }
            var listReport_Vm =  _reportApiClient.GetAll(Token).Result;
            return View(listReport_Vm.ToPagedList(pageNumber, pageSize));
        }
        public async Task<IActionResult> Detail_Partial(int id)
        {
            var Token = HttpContext.Session.GetString("Token");
            if (_userApiClient.CheckToken(Token, User.Identity.Name) == null)
                return Redirect("/Administration/Home/Login");
            else
            {
                if (Token == null)
                    Token = await _userApiClient.CheckToken(Token, User.Identity.Name);
            }
            var listReport = _reportApiClient.GetAll(Token).Result.Where(x => x.Id == id).ToList();
            return View(listReport);
        }
        public async Task<IActionResult> Index_Partial(int? page, string name = null)
        {
            int pageNumber = page ?? 1;
            int pageSize = 6;
            var Token = HttpContext.Session.GetString("Token");
            if (_userApiClient.CheckToken(Token, User.Identity.Name) == null)
                return Redirect("/Administration/Home/Login");
            else
            {
                if (Token == null)
                    Token = await _userApiClient.CheckToken(Token, User.Identity.Name);
            }
            var listReport_Vm = _reportApiClient.GetAll(Token,name).Result;
            return View(listReport_Vm.ToPagedList(pageNumber,pageSize));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if(id!=0)
            {
                var Token = HttpContext.Session.GetString("Token");
                if (_userApiClient.CheckToken(Token, User.Identity.Name) == null)
                    return Redirect("/Administration/Home/Login");
                else
                {
                    if (Token == null)
                        Token = await _userApiClient.CheckToken(Token, User.Identity.Name);
                }
                var result = await _reportApiClient.Delete(Token, id);
                if (result > 0) return Content("Success");
            }
            return Content("Error");
        }
        public async Task<IActionResult> CreateReply(int idUser,int idVideo,string content)
        {
            var video = _videoService.FinVideoAsync(idVideo).Result;
            var request= new NotificationRequest();
            request.FromUserId = idUser;
            request.PoterImg = video.PosterImg;
            request.Content = content;
            request.VideoId = video.Id;
            var user = _userService.FindUser(User.Identity.Name).Result;
            if (user != null)
            {
                var result = await _notificationService.CreateReplyReport(request,user.Id);
                if (result > 0) return Content("Success");
            }
            return Content("Error");
        }
    }
}
