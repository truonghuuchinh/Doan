using DoanApp.Commons;
using DoanApp.Models;
using DoanApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DoanApp.Controllers
{
    [Authorize]
    public class VideoWatchedController : Controller
    {
        private readonly IVideoWatchedService _videoWatched;
        private readonly IVideoService _videoService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IPlayListService _playListService;
        public VideoWatchedController(IVideoWatchedService video,IVideoService videoService
            ,IUserService userService,INotificationService notificationService,
            IPlayListService playlist)
        {
            _videoWatched = video;
            _videoService = videoService;
            _userService = userService;
            _notificationService = notificationService;
            _playListService = playlist;
        }
        public IActionResult Index(int? page)
        {

            var user = UserAuthenticated.GetUser(User.Identity.Name);
            ViewBag.UserFollow = _userService.GetUserFollow(user.UserName);
            ViewBag.IdUser = user.Id;
            ViewBag.PlayList = _playListService.GetAll().Where(x => x.UserId == ViewBag.IdUser).ToList();
            ViewBag.ForCus = 4;
            GetNotification();
            return View(GetVideo_Vm(page));
        }
       
        public IActionResult VideoWatched_Partial(int? page)
        {
            return View(GetVideo_Vm(page));
        }
        public IPagedList<Video_vm> GetVideo_Vm(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 5;
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            if (user != null)
            {
                var listWatched = _videoWatched.GetAll().Where(x => x.UserId == user.Id).OrderByDescending(x=>x.Id).ToList();
                var video = (from watched in listWatched
                             join videos in _videoService.GetAll() on watched.VideoId equals videos.Id
                             select videos).ToList();
                var listVideoVm = _videoService.GetVideo_Vm(video, _userService.GetAll()).ToPagedList(pageNumber, pageSize);
                return listVideoVm;
            }
            return null;
          
        }
        [HttpPost]
        public async Task<IActionResult> Create(VideoWatchedRequest request)
        {
            if (request != null)
            {
                var result =await _videoWatched.Create(request);
                if (result > 0)
                {
                    return Content("Success");
                }
            }
            return Content("Error");
        }
        public void GetNotification()
        {
            var userss = UserAuthenticated.GetUser(User.Identity.Name);
            if (userss != null)
            {
                ViewBag.ListNotification = _notificationService.GetNotification(userss);
                ViewBag.CountNotifi = _notificationService.GetNotification(userss).Where(x => x.Watched).Count();
            }
            else
            {
                ViewBag.ListNotification = null;
                ViewBag.CountNotifi = 0;
            }

        }
        public async Task<IActionResult> Delete(VideoWatchedRequest resquest)
        {
          
            if (resquest != null)
            {
                var watched =  await _videoWatched.FindAsync(resquest);
                var result =await _videoWatched.Delete(watched.Id);
                if (result > 0) return Content("Success");
            }
            return Content("Error");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteFavorite(LikeVideoRequest resquest)
        {

            if (resquest != null)
            {
                var Like = await _videoWatched.FindFavorite(resquest);
                var result = await _videoWatched.DeleteFavorite(Like.Id);
                if (result > 0) return Content("Success");
            }
            return Content("Error");
        }
    }
}
