using DoanApp.Areas.Administration.Models;
using DoanApp.Commons;
using DoanApp.Models;
using DoanApp.Services;
using Microsoft.AspNetCore.Authorization;
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
        public ReportVideoController(IReportVideoService reportVideoService,IVideoService videoService,
            IUserService user,INotificationService notification)
        {
            _reportVideoService = reportVideoService;
            _videoService = videoService;
            _userService = user;
            _notificationService = notification;
        }
        public IActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageNumber = page ?? 1;
            int pageSize = 6;
            ViewBag.Active = 4;
            var listReport_Vm = _reportVideoService.GetList_Vm();
            return View(listReport_Vm.ToPagedList(pageNumber, pageSize));
        }
        public IActionResult Detail_Partial(int id)
        {
            var listReport = _reportVideoService.GetList_Vm().Where(x => x.Id == id).ToList();
            return View(listReport);
        }
        public IActionResult Index_Partial(int? page, string name = null)
        {
            int pageNumber = page ?? 1;
            int pageSize = 6;
            var listReport = _reportVideoService.GetList_Vm();
            if (name != null)
            {
                name = ConvertUnSigned.convertToUnSign(name).ToLower();

                listReport = listReport.Where(x => ConvertUnSigned.convertToUnSign(x.Content).
                  ToLower().Contains(name)|| ConvertUnSigned.convertToUnSign(x.NamUser).
                  ToLower().Contains(name)|| ConvertUnSigned.convertToUnSign(x.NameVideo).
                  ToLower().Contains(name)).ToList();
            }
            var listVm = listReport.ToPagedList(pageNumber, pageSize);
           
            return View(listVm);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if(id!=0)
            {
                var result = await _reportVideoService.Delete(id);
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
