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
        public VideoController(IVideoService videoService, IUserService userService,
            ICategoryService category,IFollowChannelService channelService,ILikeVideoService likeservice,
            IFollowChannelService followChannel,ICommentService commentService)
        {
            _videoService = videoService;
            _userService = userService;
            _categoryService = category;
            _channelService = channelService;
            _likeService = likeservice;
            _followChannel = followChannel;
            _commentService = commentService;
        }
        // GET: VideoController
        public ActionResult Index()
        {

            return View();
        }
        public IActionResult FavoritedVideo(int? page,bool flag=false)
        {
            if (page == null) page = 1;
            var pageNumber = page ?? 1;
            var pageSize = 6;
            IPagedList<Video_vm> listVideo_Vm;
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
            return View();
        }
        public IActionResult SubscriptionChannel(int? page,bool flag=false)
        {
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
            return View();
        }
        public IActionResult OverviewPage()
        {
            var user = _userService.FindUser(User.Identity.Name).Result;
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
            ViewData["Category"] = new SelectList(_categoryService.GetAll().Result, "Id", "Name");
           
            if (page == null) page = 1;
            var pageSize = 7;
            var pageNumber = page ?? 1;
            var user = _userService.FindUser(User.Identity.Name).Result;
            var list = _videoService.GetAll().
                Where(x=>x.AppUserId==user.Id).OrderBy(x=>x.Id).ToPagedList(pageNumber,pageSize);
            ViewBag.CountComment = (from video in _videoService.GetAll().Where(x => x.AppUserId == user.Id)
                                    join comment in _commentService.GetAll() on video.Id equals comment.VideoId
                                    select comment).Count();
            return View(list);
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
                    if (result > 0)
                    {
                        return Redirect("MyChannel");
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
       
    }
}
