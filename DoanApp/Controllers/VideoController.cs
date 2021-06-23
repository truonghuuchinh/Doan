using DoanApp.Commons;
using DoanApp.Models;
using DoanApp.Services;
using DoanData.Models;
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
        private readonly IVideoWatchedService _videoWatched;
        private readonly IPlayListService _playListService;
        private readonly IDetailVideoService _detailService;
        
        public VideoController(IVideoService videoService, IUserService userService,
            ICategoryService category,IFollowChannelService channelService,ILikeVideoService likeservice,
            IFollowChannelService followChannel,ICommentService commentService,
            INotificationService notification,IVideoWatchedService videoWatched,
            IPlayListService playList,IDetailVideoService detail)
        {
            _videoService = videoService;
            _userService = userService;
            _categoryService = category;
            _channelService = channelService;
            _likeService = likeservice;
            _followChannel = followChannel;
            _commentService = commentService;
            _notificationService = notification;
            _videoWatched = videoWatched;
            _playListService = playList;
            _detailService = detail;
        }
        // GET: VideoController

        public ActionResult MyPage(int? page,int? idUser)
        {
            GetNotificationHome();
            
            var pageNumber = page ?? 1;
            List<Video_vm> listVideo_Vm;
            var listVideo = new List<Video>();
            var users = UserAuthenticated.GetUser(User.Identity.Name);
            ViewBag.UserFollow = _userService.GetUserFollow(users.UserName);
            ViewBag.IdUser = users.Id;
            ViewBag.PlayList = _playListService.GetAll().Where(x => x.UserId == ViewBag.IdUser).ToList();
            if (idUser != null)
            {
                
                var user =  _userService.FindUserId((int)idUser).Result;
                
                ViewBag.CountUserFollow = _followChannel.GetAll().Where(x => x.ToUserId == user.Id).Count();
                ViewBag.UserMyPage = user;
                listVideo = _videoService.GetAll().Where(x => x.AppUserId == idUser).ToList();
            }
            else
            {
                if (page == null) page = 1;
                ViewBag.CountUserFollow = _followChannel.GetAll().Where(x => x.ToUserId == users.Id).Count();
                ViewBag.UserMyPage = users;
                listVideo = _videoService.GetAll().Where(x => x.AppUserId == users.Id).ToList();
            }
            if (idUser != users.Id&&idUser!=null) ViewBag.FollowUser = true;
            else ViewBag.FollowUser = false;
            ViewBag.BottomPage = true;
           var listUser = _userService.GetAll();
            listVideo_Vm = _videoService.GetVideo_Vm(listVideo, listUser).
                OrderByDescending(x => x.Id).Where(x => x.Status & x.HidenVideo).ToList();
            return View(listVideo_Vm.ToPagedList(pageNumber, 8));
        }
        public IActionResult GetAllPlayList(int id,string nameSearch=null)
        {
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            if (id != 0)
            {
                var playlist = _detailService.GetAll().Where(x => x.PlayListId == id).ToList();
                var listVideo = (from plist in playlist
                                join lvideo in _videoService.GetAll() on plist.VideoId equals lvideo.Id
                                select lvideo).ToList();
                if (nameSearch != null)
                {
                    nameSearch = ConvertUnSigned.convertToUnSign(nameSearch).ToLower();
                    listVideo = listVideo.Where(x => ConvertUnSigned.convertToUnSign(x.Name).
                    ToLower().Contains(nameSearch)).ToList();
                }
                    
                if (listVideo.Count > 0)
                {
                    return Content(JsonConvert.SerializeObject(_videoService.GetAllVideoPlayList(listVideo, user)));
                }
                else return Content("null");
            }
          
            return null;
        }
        public IActionResult MyPlayList()
        {
            GetNotificationHome();
            ViewBag.ForCus = 2;
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            var playlist = _playListService.GetAll().Where(x => x.UserId == user.Id).ToList();
            var detailPlayList = (from plist in playlist
                                  join detail in _detailService.GetAll() on plist.Id equals detail.PlayListId
                                  select new
                                  {
                                      plist.Id,
                                      plist.UserId,
                                      plist.Name,
                                      plist.Status,
                                      detail.VideoId,
                                      plist.CreateDate
                                  }).ToList();
            var listCountItem = from detail in detailPlayList
                                group detail by detail.Id into grp
                                select new
                                {
                                    Key=grp.Key,
                                    Count=grp.Count()
                                };
            var detailPlayListVideo = from dlist in detailPlayList
                                      join video in _videoService.GetAll() on dlist.VideoId equals video.Id
                                      select new { dlist,video.PosterImg} ;
            var listComplete = (from countItem in listCountItem
                               join detail in detailPlayListVideo on countItem.Key equals detail.dlist.Id
                               select new
                               {
                                   Count=countItem.Count,
                                   detail.dlist.Id,
                                   detail.dlist.Name,
                                   detail.dlist.Status,
                                   detail.dlist.UserId,
                                   detail.dlist.VideoId,
                                   detail.PosterImg
                                   ,detail.dlist.CreateDate
                               }).ToList();
            var listDetail_vm = new List<DetailPlayListVideo>();
           
                foreach (var item in listComplete)
                {
                    
                    var i = new DetailPlayListVideo();
                    i.Id = item.Id;
                    i.VideoId = item.VideoId;
                    i.Status = item.Status;
                    i.UserId = item.UserId;
                    i.Name = item.Name;
                    i.PosterVideo = item.PosterImg;
                    i.CountItem = item.Count;
                    i.CreateDate = item.CreateDate;
                    if (listDetail_vm.Count > 0)
                    {
                        if (!listDetail_vm.Any(x=>x.Id==item.Id))
                            listDetail_vm.Add(i);
                    }
                    else listDetail_vm.Add(i);
                }
                var playlistNoVideo = _playListService.GetAll().Where(x => !_detailService.GetAll().Any(y => y.PlayListId == x.Id)&&x.UserId==user.Id).ToList();
                var playlistNovideo_vm = new List<DetailPlayListVideo>();
                foreach (var item in playlistNoVideo)
                {
                    var i = new DetailPlayListVideo();
                    i.Id = item.Id;
                    i.Name = item.Name;
                    i.Status = item.Status;
                    i.UserId = item.UserId;
                    i.VideoId = 0;
                    i.PosterVideo = null;
                    i.CreateDate = item.CreateDate;
                    i.CountItem = 0;
                    playlistNovideo_vm.Add(i);
                }
            listDetail_vm.AddRange(playlistNovideo_vm);
            var listvm = listDetail_vm.OrderByDescending(x => x.Id).ToList();
                return View(listvm.ToPagedList(1,4));
        }
        public IActionResult MyIntroduction(int? idUser)
        {
            GetNotificationHome();
            var users = UserAuthenticated.GetUser(User.Identity.Name);
            ViewBag.UserFollow = _userService.GetUserFollow(users.UserName);
            if (idUser != null)
            {
                var user = _userService.FindUserId((int)idUser).Result;
                ViewBag.CountUserFollow = _followChannel.GetAll().Where(x => x.ToUserId == user.Id).Count();
                ViewBag.UserMyPage = user;
                
                ViewBag.CountView = _videoService.GetAll().Where(x => x.AppUserId == user.Id).Sum(x => x.ViewCount);
            }
            else
            {
                
                ViewBag.CountUserFollow = _followChannel.GetAll().Where(x => x.ToUserId == users.Id).Count();
                ViewBag.UserMyPage = users;
                ViewBag.CountView = _videoService.GetAll().Where(x => x.AppUserId == users.Id).Sum(x => x.ViewCount);
            }
            if (idUser != users.Id) ViewBag.FollowUser = true;
           else ViewBag.FollowUser = false;
            ViewBag.BottomPage = true;
            return View();
        }
        public ActionResult MyPage_Partial(int? id,int? page,string nameSearch="null")
        {
            if (id == null) id = 0;
            var user = _userService.FindUserId((int)id).Result;
            if (page == null) page = 1;
            var pageNumber = page ?? 1;
            List<Video_vm> listVideo_Vm;
            
            var listVideo = _videoService.GetAll().Where(x => x.AppUserId == user.Id).ToList();
            if (nameSearch != "null") 
                listVideo = listVideo.Where(x => x.Name.Contains(nameSearch)).ToList();
             var listUser = _userService.GetAll();
            listVideo_Vm = _videoService.GetVideo_Vm(listVideo, listUser).
                OrderByDescending(x => x.Id).Where(x => x.Status & x.HidenVideo).ToList();
          
            ViewBag.CountUserFollow = _followChannel.GetAll().Where(x => x.FromUserId == user.Id).Count();
            ViewBag.UserMyPage = user;
            return View(listVideo_Vm.ToPagedList(pageNumber, 8));
        }
        public string ListVideoJson(int id)
        {
            var listName = new List<string>();
            var listVideo = _videoService.GetAll().Where(x => x.AppUserId == id).ToList();
            foreach (var item in listVideo)
            {
                listName.Add(item.Name);
            }
            return JsonConvert.SerializeObject(listName);
        }
        public IActionResult FavoritedVideo(int? page,bool flag=false)
        {
            ViewBag.ForCus = 6;
            if (page == null) page = 1;
            var pageNumber = page ?? 1;
            var pageSize = 6;
            IPagedList<Video_vm> listVideo_Vm;
            GetNotificationHome();
            if (User.Identity.IsAuthenticated)
            {
                var userLogin = UserAuthenticated.GetUser(User.Identity.Name);
                ViewBag.UserFollow = _userService.GetUserFollow(userLogin.UserName);
                ViewBag.IdUser = userLogin.Id;
                ViewBag.PlayList = _playListService.GetAll().Where(x => x.UserId == ViewBag.IdUser).ToList();
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
            ViewBag.ForCus = 3;
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            ViewBag.UserFollow = _userService.GetUserFollow(user.UserName);
            ViewBag.IdUser = user.Id;
            ViewBag.PlayList = _playListService.GetAll().Where(x => x.UserId == ViewBag.IdUser).ToList();
            ViewBag.UserLibary = user;
            GetNotificationHome();
            if (user != null)
            {
              //Video watched
                var listVideoWatched = _videoWatched.GetAll().
                    OrderByDescending(X => X.Id).Where(x=>x.UserId==user.Id).Take(8).ToList();

                var listvideo = (from video in _videoService.GetAll()
                                    join watched in listVideoWatched on video.Id equals watched.VideoId
                                    select video).ToList();
                var listvideo_vm = _videoService.GetVideo_Vm(listvideo, _userService.GetAll()).
                    OrderByDescending(x=>x.Id).ToPagedList(1,6);
                ViewBag.CountWatched = listVideoWatched.Count;
                //-kết thúc
                //Video đã thích
                var listFovarited = _likeService.GeAll().Where(x => x.UserId == user.Id).ToList();
                var listvideos = (from fovarited in listFovarited
                                 join video in _videoService.GetAll() on fovarited.VideoId equals video.Id
                                 select video).ToList();
                var listVideoVm = _videoService.GetVideo_Vm(listvideos, _userService.GetAll());
                ViewBag.ListFovarited = listVideoVm.ToPagedList(1, 3);
                ViewBag.CountFovarited = listFovarited.Count;
                //kết thúc
                ViewBag.CountUserFollow = _followChannel.GetAll().Where(x => x.FromUserId == user.Id).Count();
                ViewBag.CountVideoUpload = _videoService.GetAll().Where(x => x.AppUserId == user.Id).Count();
                return View(listvideo_vm);
            }
            return View(null);
        }
        public IActionResult SubscriptionChannel(int? page,bool flag=false)
        {
            ViewBag.ForCus = 2;
            GetNotificationHome();
            if (page == null) page = 1;
            var pageNumber = page ?? 1;
            var pageSize = 12;
            IPagedList<Video_vm> listVideo_Vm;
            if (User.Identity.IsAuthenticated)
            {
                var userLogin = UserAuthenticated.GetUser(User.Identity.Name);
                ViewBag.UserFollow = _userService.GetUserFollow(userLogin.UserName);
                ViewBag.IdUser = userLogin.Id;
                ViewBag.PlayList = _playListService.GetAll().Where(x => x.UserId == ViewBag.IdUser).ToList();
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
            var userLogin = UserAuthenticated.GetUser(User.Identity.Name);
            ViewBag.IdUser = userLogin.Id;
            ViewBag.PlayList = _playListService.GetAll().Where(x => x.UserId == ViewBag.IdUser).ToList();
            GetNotificationHome();
            return View();
        }
        public IActionResult OverviewPage()
        {
            GetNotificationHome();
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            ViewBag.CountUserFollow = _followChannel.GetAll().Where(x => x.ToUserId == user.Id).Count();
            ViewBag.CountView = _videoService.GetAll().Where(x => x.AppUserId == user.Id).Sum(x => x.ViewCount);
            ViewBag.CountVideo = _videoService.GetAll().Where(x => x.AppUserId == user.Id).Count();
            ViewBag.CountComment = (from video in _videoService.GetAll().Where(x => x.AppUserId == user.Id)
                                    join comment in _commentService.GetAll() on video.Id equals comment.VideoId
                                    select comment).Count();
            ViewBag.CountLike = (from video in _videoService.GetAll().Where(x => x.AppUserId == user.Id)
                                 join like in _likeService.GeAll() on video.Id equals like.VideoId
                                 where like.Reaction == "Like"
                                 select like).Count();
            ViewBag.ForCus = 1;
            return View();
        }
        public IActionResult MyChannel(int? page)
        {
            GetNotificationHome();
            ViewData["Category"] = new SelectList(_categoryService.GetAll().Result, "Id", "Name");
            ViewBag.ForCus = 5;
            if (page == null) page = 1;
            var pageSize = 7;
            var pageNumber = page ?? 1;
            var user = _userService.FindUser(User.Identity.Name).Result;
            var list = _videoService.GetAll().
                Where(x=>x.AppUserId==user.Id).OrderByDescending(x=>x.Id).ToPagedList(pageNumber,pageSize);
            var listCountComment = (from video in _videoService.GetAll().Where(x => x.AppUserId == user.Id)
                                    join comment in _commentService.GetAll() on video.Id equals comment.VideoId
                                    orderby video.Id, comment.Id descending
                                    group comment by comment.VideoId into grp
                                    select new { Key = grp.Key, Count = grp.Count() }).ToList();
            var listCount = new List<CountComment>();
            foreach (var item in listCountComment)
            {
                var  count= new CountComment();
                count.Id = item.Key;
                count.Count = item.Count;
                listCount.Add(count);
            }
            ViewBag.CountComment = listCount;
            ViewBag.ForCus = 0;
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
        public async Task<IActionResult> UpdatePermission(string data) {
            var video = JsonConvert.DeserializeObject<VideoRequest>(data);
            var result = await _videoService.UpdatePermission(video);
            if (result > 0)
            {
                var getVideo = await _videoService.FinVideoAsync(video.Id);
                return Content("Success");
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
                if (result > 0)
                {
                    new UserAuthenticated().UpdateImgChannel(user.Id, name, null);
                    return RedirectToAction("MyPage", "Video");
                }
            }
            return RedirectToAction("MyPage", "Video");
        }
    }
}
