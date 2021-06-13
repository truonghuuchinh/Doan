using DoanApp.Commons;
using DoanApp.Models;
using DoanApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DoanApp.Controllers
{
    [Authorize]
    public class VideoController : Controller
    {
        private readonly IVideoService _videoService;
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly IFollowChannelService _channelService;
        private readonly ILikeVideoService _likeService;
        private readonly IFollowChannelService _followChannel;
        private readonly ICommentService _commentService;
        private readonly INotificationService _notificationService;
        public VideoController(IVideoService videoService, IUserService userService,
            ICategoryService category,IFollowChannelService channelService,ILikeVideoService likeservice,
            IFollowChannelService followChannel,ICommentService commentService,
            INotificationService notification)
        {
            _videoService = videoService;
            _userService = userService;
            _categoryService = category;
            _channelService = channelService;
            _likeService = likeservice;
            _followChannel = followChannel;
            _commentService = commentService;
            _notificationService = notification;
        }
        // GET: VideoController
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult MyPage(int? page)
        {
            GetNotificationHome();
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            if (page == null) page = 1;
            var pageNumber = page ?? 1;
            List<Video_vm> listVideo_Vm;
            var listVideo = _videoService.GetAll().Where(x=>x.AppUserId==user.Id).ToList();
            var listUser = _userService.GetAll();
            listVideo_Vm = _videoService.GetVideo_Vm(listVideo, listUser).
                OrderByDescending(x => x.Id).Where(x => x.Status & x.HidenVideo).ToList();
            
            ViewBag.CountUserFollow = _followChannel.GetAll().Where(x => x.FromUserId == user.Id).Count();
            ViewBag.UserMyPage = user;
            return View(listVideo_Vm.ToPagedList(pageNumber, 8));
        }
        public ActionResult MyPage_Partial(int? page,string nameSearch=null)
        {
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            if (page == null) page = 1;
            var pageNumber = page ?? 1;
            List<Video_vm> listVideo_Vm;
            var listVideo = _videoService.GetAll().Where(x => x.AppUserId == user.Id).ToList();
            if (nameSearch != null) 
                listVideo = listVideo.Where(x => x.Name.Contains(nameSearch)).ToList();
             var listUser = _userService.GetAll();
            listVideo_Vm = _videoService.GetVideo_Vm(listVideo, listUser).
                OrderByDescending(x => x.Id).Where(x => x.Status & x.HidenVideo).ToList();
          
            ViewBag.CountUserFollow = _followChannel.GetAll().Where(x => x.FromUserId == user.Id).Count();
            ViewBag.UserMyPage = user;
            return View(listVideo_Vm.ToPagedList(pageNumber, 8));
        }
        public string ListVideoJson()
        {
            var listName = new List<string>();
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            var listVideo = _videoService.GetAll().Where(x => x.AppUserId == user.Id).ToList();
            foreach (var item in listVideo)
            {
                listName.Add(item.Name);
            }
            return JsonConvert.SerializeObject(listName);
        }
        public IActionResult FavoritedVideo(int? page,bool flag=false)
        {

            if (page == null) page = 1;
            var pageNumber = page ?? 1;
            var pageSize = 6;
            IPagedList<Video_vm> listVideo_Vm;
            GetNotificationHome();
            if (User.Identity.IsAuthenticated)
            {
                var userLogin = UserAuthenticated.GetUser(User.Identity.Name);
                var listLikeVideo = _likeService.GeAll().Where(x=>x.UserId==userLogin.Id&&x.Reaction=="Like").ToList();
                var listVideo = (from video in _videoService.GetAll()
                                 join like in listLikeVideo on video.Id equals like.VideoId
                                 select video).ToList();
                var listUser = _userService.GetAll();
                listVideo_Vm = _videoService.GetVideo_Vm(listVideo, listUser).
                    OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize);
                if (flag)
                {
                    foreach (var item in listVideo_Vm)
                    {
                        item.CreateDate = CaculatorHours.Caculator(item.CreateDate);
                    }
                    return Content(JsonConvert.SerializeObject(listVideo_Vm));
                }
                return View(listVideo_Vm);
            }
            listVideo_Vm = null;
            return View(listVideo_Vm);
        }
        public IActionResult LibaryVideo()
        {
            GetNotificationHome();
            return View();
        }
        public IActionResult SubscriptionChannel(int? page,bool flag=false)
        {
            GetNotificationHome();
            if (page == null) page = 1;
            var pageNumber = page ?? 1;
            var pageSize = 12;
            IPagedList<Video_vm> listVideo_Vm;
            if (User.Identity.IsAuthenticated)
            {
                var userLogin = UserAuthenticated.GetUser(User.Identity.Name);
                var listFollow = _channelService.GetAll().Where(x => x.FromUserId == userLogin.Id).ToList();
                var listVideo = (from video in _videoService.GetAll()
                                 join follow in listFollow on video.AppUserId equals follow.ToUserId
                                 select video).ToList();
                var listUser = _userService.GetAll();
                listVideo_Vm = _videoService.GetVideo_Vm(listVideo, listUser).
                    OrderByDescending(x=>x.Id).ToPagedList(pageNumber, pageSize);
                if (flag)
                {
                    foreach (var item in listVideo_Vm)
                    {
                        item.CreateDate = CaculatorHours.Caculator(item.CreateDate);
                    }
                    return Content(JsonConvert.SerializeObject(listVideo_Vm));
                }
                return View(listVideo_Vm);
            }
            listVideo_Vm = null;
           return View(listVideo_Vm);
        }
        public IActionResult VideoWatched()
        {
            GetNotificationHome();
            return View();
        }
        public IActionResult OverviewPage()
        {
            GetNotificationHome();
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            ViewBag.CountUserFollow = _followChannel.GetAll().Where(x => x.FromUserId == user.Id).Count();
            ViewBag.CountView = _videoService.GetAll().Where(x => x.AppUserId == user.Id).Sum(x => x.ViewCount);
            ViewBag.CountVideo = _videoService.GetAll().Where(x => x.AppUserId == user.Id).Count();
            ViewBag.CountComment = (from video in _videoService.GetAll().Where(x => x.AppUserId == user.Id)
                                    join comment in _commentService.GetAll() on video.Id equals comment.VideoId
                                    select comment).Count();
            ViewBag.CountLike = (from video in _videoService.GetAll().Where(x => x.AppUserId == user.Id)
                                 join like in _likeService.GeAll() on video.Id equals like.VideoId
                                 where like.Reaction == "Like"
                                 select like).Count();
            return View();
        }
        public IActionResult MyChannel(int? page)
        {
            GetNotificationHome();
            ViewData["Category"] = new SelectList(_categoryService.GetAll().Result, "Id", "Name");
           
            if (page == null) page = 1;
            var pageSize = 7;
            var pageNumber = page ?? 1;
            var user = _userService.FindUser(User.Identity.Name).Result;
            var list = _videoService.GetAll().
                Where(x=>x.AppUserId==user.Id).OrderByDescending(x=>x.Id).ToPagedList(pageNumber,pageSize);
            ViewBag.CountComment = (from video in _videoService.GetAll().Where(x => x.AppUserId == user.Id)
                                    join comment in _commentService.GetAll() on video.Id equals comment.VideoId
                                    select comment).Count();
            return View(list);
        }
        public void GetNotificationHome()
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
        public IActionResult MyChannel_Partial(int? page)
        {
            if (page == null) page = 1;
            var pageSize = 7;
            var pageNumber = page ?? 1;
            var user = _userService.FindUser(User.Identity.Name).Result;
            var list = _videoService.GetAll().
                Where(x => x.AppUserId == user.Id).OrderBy(x => x.Id).ToPagedList(pageNumber, pageSize);
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(VideoRequest videoRequest,
            IFormFile PosterVideo,IFormFile LinkVideo,string HiddenVideo)
        {
            List<IFormFile> listPost = new List<IFormFile>();
            var user = await _userService.FindUser(User.Identity.Name);
            if (ModelState.IsValid)
            {
                if (PosterVideo != null && LinkVideo != null)
                {
                    listPost.Add(PosterVideo);
                    listPost.Add(LinkVideo);
                }
                if (user != null)
                {
                    videoRequest.AppUserId = user.Id;
                    videoRequest.HidenVideo = HiddenVideo.Contains("Public") ? true : false;
                    var result = await _videoService.Create(videoRequest,listPost);
                    if (result!=null)
                    {
                        var notifi = new NotificationRequest();
                        notifi.AvartarUser = user.Avartar;
                        notifi.Content = videoRequest.Name;
                        notifi.PoterImg = result.PosterImg;
                        notifi.UserId = user.Id;
                        notifi.VideoId = result.Id;
                        notifi.LoginExternal = user.LoginExternal;
                        notifi.UserName = user.FirtsName + " " + user.LastName;
                        var resultsNoti = await _notificationService.Create(notifi,user.Id);
                        if(resultsNoti>0) return Redirect("MyChannel");
                    }
                }
            }
            return Redirect("MyChannel");
        }

        // POST: VideoController/Edit/5
        [AllowAnonymous]
        public async Task<ActionResult> UpdateView(int id)
        {
            if (id != 0)
            {
                var result =await _videoService.UpdateView(id);
                var video = await _videoService.FinVideoAsync(id);
                return Content(video.ViewCount.ToString("#,##0"));
            }
            return Content("Error");
        }
        
        [HttpPost]
        public async Task<IActionResult> Update(string data)
        {
            var video = JsonConvert.DeserializeObject<VideoRequest>(data);
            var result = await _videoService.Update(video);
            if (result > 0)
            {
                var getVideo = await _videoService.FinVideoAsync(video.Id);
                return Content(JsonConvert.SerializeObject(getVideo));
            }
            return Content("Error");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _videoService.Delete(id);
            if (result > 0) return Content("Success");
            return Content("Error");
        }
        [HttpPost]
        public async Task<IActionResult> UploadPoster(IFormFile fileUpload,int idVideo)
        {
            if (fileUpload != null && idVideo != 0)
            {
                var video = await _videoService.FinVideoAsync(idVideo);
                System.IO.File.Delete("wwwroot/Client/imgPoster/" + video.PosterImg);
                var filename = fileUpload.FileName.Split('.');
                var name = video.Id.ToString() + "." + filename[filename.Length - 1].ToLower();
                video.PosterImg = name;
                using (var fileStream = new FileStream(Path.Combine("wwwroot/Client/imgPoster", name),
                           FileMode.Create, FileAccess.Write))
                {

                    fileUpload.CopyTo(fileStream);
                }
                var videoRequest = new VideoRequest();
                videoRequest.PosterVideo = video.PosterImg;
                videoRequest.Description = "";
                videoRequest.Name = "";
                videoRequest.Id = idVideo;
                var result = await _videoService.Update(videoRequest);
                if (result!=-1) return Redirect("MyChannel");
            }
           
            return Content("Error");
        }
        public async Task<IActionResult> UpdateCategory(int id,int idCategory)
        {
            var result = await _videoService.UpdateCategory(id, idCategory);
            if (result!= -1)
                return Content("Success");
            return Content("Error");
        }
       [HttpPost]
       public async Task<IActionResult> UpdateImgChannel(string emailUser,IFormFile fileUpload)
        {
            if (emailUser != null && fileUpload != null)
            {
                var user = await _userService.FindUser(emailUser);
               if(user.ImgChannel!=null) 
                    System.IO.File.Delete("wwwroot/Client/imgChannel/" + user.ImgChannel);
                var filename = fileUpload.FileName.Split('.');
                var name = user.Id.ToString() + "." + filename[filename.Length - 1].ToLower();
                user.ImgChannel = name;
                using (var fileStream = new FileStream(Path.Combine("wwwroot/Client/imgChannel", name),
                           FileMode.Create, FileAccess.Write))
                {
                    fileUpload.CopyTo(fileStream);
                }
                var result =await _userService.UpdateImgChannel(user.Id, name);
                if(result>0) return RedirectToAction("MyPage", "Video");
            }
            return RedirectToAction("MyPage", "Video");
        }
    }
}
