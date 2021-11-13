using DoanApp.Commons;
using DoanApp.Models;
using DoanData.DoanContext;
using DoanData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Newtonsoft.Json;
using DoanApp.Services;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;
using DoanApp.ServiceApi;
using DoanApp.Areas.Administration.Models;

namespace DoanApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IVideoService _videoService;
        private readonly ICommentService _commentService;
        private readonly ILikeVideoService _likeVideo;
        private readonly IFollowChannelService _channelService;
        private readonly ICategoryService _categoryService;
        private readonly IPlayListService _playListService;
        private readonly IDetailVideoService _detaivideo;
        private readonly INotificationService _notificationService;
        private readonly IUserApiCient _userApiClient;
        static int countLockout = 0;
        static string userEmail = "";
        public HomeController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IUserService userService, IVideoService videoService,
            ICommentService commentService, ILikeVideoService likeVideo,
            IFollowChannelService channelService, ICategoryService categoryService,
            IPlayListService playListService, IDetailVideoService detailVideo,
            INotificationService notificationService, IUserApiCient userApiClient
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _videoService = videoService;
            _userService = userService;
            _commentService = commentService;
            _likeVideo = likeVideo;
            _channelService = channelService;
            _categoryService = categoryService;
            _playListService = playListService;
            _detaivideo = detailVideo;
            _notificationService = notificationService;
            _userApiClient = userApiClient;

        }
        public async Task<IActionResult> Index(int? page)
        {
            ViewBag.ForCus = 0;
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            if (User.Identity.IsAuthenticated && user != null)
            {
                GetNotificationHome();

                ViewBag.IdUser = user.Id;
                ViewBag.PlayList = _playListService.GetAll().Where(x => x.UserId == ViewBag.IdUser).ToList();
                ViewBag.UserFollow = _userService.GetUserFollow(user.UserName);
            }
            else
            {
                ViewBag.IdUser = 0;
                ViewBag.PlayList = null;
                ViewBag.ListNotification = null;
                ViewBag.CountNotifi = 0;
                ViewBag.UserFollow = _userService.GetChannel();
            }
            ViewBag.LinkActive = "/Home/Index";
            if (page == null) page = 1;
            var pageNumber = page ?? 1;
            List<Video_vm> listVideo_Vm;
            var listVideo = _videoService.GetAll();
            var listUser = _userService.GetAll();
            listVideo_Vm = _videoService.GetVideo_Vm(listVideo, listUser).
                OrderByDescending(x => x.Id).Where(x => x.HidenVideo).ToList();
            ViewBag.Search_Video = new SelectList(listVideo, "Id", "Name");
            return View(await listVideo_Vm.ToPagedListAsync(pageNumber, 12));
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
        public IActionResult Index_Partial(int? page, string nameSearch = null)
        {
            if (page == null) page = 1;
            var pageNumber = page ?? 1;
            var listVideo_Vm = new List<Video_vm>();
            var listVideo = _videoService.GetAll();
            var listUser = _userService.GetAll();
            listVideo_Vm = _videoService.GetVideo_Vm(listVideo, listUser).
                OrderByDescending(x => x.Id).Where(x => x.HidenVideo).ToList();
            if (nameSearch != null)
            {
                nameSearch = nameSearch.ToLower();
                listVideo_Vm = listVideo_Vm.Where(x => x.Name.ToLower().Contains(nameSearch)).ToList();
            }
            return View(listVideo_Vm.ToPagedList(pageNumber, 12));
        }

        public IActionResult Popular(int? page)
        {
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            if (user != null)
            {
                ViewBag.UserFollow = _userService.GetUserFollow(user.UserName);
                ViewBag.IdUser = user.Id;
                ViewBag.PlayList = _playListService.GetAll().Where(x => x.UserId == ViewBag.IdUser).ToList();
            }
            else
            {
                ViewBag.UserFollow = _userService.GetChannel();
            }
            ViewBag.ForCus = 1;
            GetNotificationHome();
            if (page == null) page = 1;
            int pageNumber = page ?? 1;
            int pageSize = 5;
            var listVideo = _videoService.GetAll().OrderByDescending(x => x.ViewCount).
                Where(x => x.ViewCount > 0).ToList();
            var listUser = _userService.GetAll();
            var listvideo_vm = _videoService.GetVideo_Vm(listVideo, listUser).ToPagedList(pageNumber, pageSize);
            return View(listvideo_vm);
        }
        public IActionResult Popular_Partial(int? page)
        {
            if (page == null) page = 1;
            int pageNumber = page ?? 1;
            int pageSize = 8;
            var listVideo = _videoService.GetAll().OrderByDescending(x => x.ViewCount).
                Where(x => x.ViewCount > 0).ToList();
            var listUser = _userService.GetAll();
            var listvideo_vm = _videoService.GetVideo_Vm(listVideo, listUser).ToPagedList(pageNumber, pageSize);
            return View(listvideo_vm);
        }
        public async Task<IActionResult> SearchVideo(int? page, string nameSearch)
        {

            ViewBag.nameSearch = nameSearch;
            var user = UserAuthenticated.GetUser(User.Identity.Name);
            if (user != null)
            {
                ViewBag.IdUser = user.Id;
                ViewBag.PlayList = _playListService.GetAll().Where(x => x.UserId == ViewBag.IdUser).ToList();
                ViewBag.UserFollow = _userService.GetUserFollow(user.UserName);
            }
            else ViewBag.UserFollow = _userService.GetChannel();

            GetNotificationHome();
            if (page == null) page = 1;
            var pageNumber = page ?? 1;
            var pageSize = 5;
            var listVideo_Vm = GetSearchVideo_vm(nameSearch).OrderByDescending(x => x.Id).ToList();
            return View(await listVideo_Vm.ToPagedListAsync(pageNumber, pageSize));
        }
        public IActionResult SearchVideo_Partial(int? page, string nameSearch = null)
        {

            if (page == null) page = 1;
            var pageNumber = page ?? 1;
            var pageSize = 5;
            var listVideo_vm = GetSearchVideo_vm(nameSearch).OrderByDescending(x => x.Id).ToList();
            return View(listVideo_vm.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNameChannel(UpdateNameChannel request)
        {
            if(ModelState.IsValid)
            {
                var result = await _userService.UpdateNameChannel(request);
                if (result > 0)
                    return Ok(200);
            }
            return new StatusCodeResult(500);
        }
        public async Task<IActionResult> UpdateDescription(AppUserRequest request)
        {
            if (request != null)
            {
                var result = await _userService.UpdateDescription(request);
                if (result > 0)
                {
                    new UserAuthenticated().UpdateImgChannel(request.Id, null, request.DescriptionChannel);
                    return Content("Success");
                }
            }
            return Content("Error");
        }
        public List<Video_vm> GetSearchVideo_vm(string nameSearch)
        {
            List<Video_vm> listVideo_Vm;
            var listVideo = _videoService.GetAll();
            var listUser = _userService.GetAll();
            listVideo_Vm = _videoService.GetVideo_Vm(listVideo, listUser).OrderByDescending(x => x.Id).
                      Where(x => x.Status && x.HidenVideo).ToList();
            if (nameSearch != null)
            {
                nameSearch = ConvertUnSigned.convertToUnSign(nameSearch).ToLower();
                var listCategory = _categoryService.GetAll().Result.
                Where(x => ConvertUnSigned.convertToUnSign(x.Name).ToLower().Contains(nameSearch)).ToList();
                if (listCategory.Count > 0)
                {
                    listVideo_Vm = (from video in listVideo_Vm
                                    join category in listCategory on video.CategorysId equals category.Id
                                    select video).ToList();
                }
                else listVideo_Vm = listVideo_Vm.Where(x => ConvertUnSigned.convertToUnSign(x.Name).ToLower().Contains(nameSearch)
                    || ConvertUnSigned.convertToUnSign(x.Description).ToLower().Contains(nameSearch)).ToList();
            }
            else listVideo_Vm = null;
            return listVideo_Vm;
        }
        public async Task<IActionResult> DetailVideo(int? id)
        {
            GetNotificationHome();
            var userFollow = "false";
            var userLogin = UserAuthenticated.GetUser(User.Identity.Name);
            if (userLogin != null)
            {
                ViewBag.UserFollow = _userService.GetUserFollow(userLogin.UserName);
                ViewBag.IdUser = userLogin.Id;
                ViewBag.PlayList = _playListService.GetAll().Where(x => x.UserId == ViewBag.IdUser).ToList();
            }
            else
            {
                ViewBag.UserFollow = _userService.GetChannel();
                ViewBag.PlayList = null;
            }
            ViewBag.UserLogin = userLogin == null ? null : userLogin;
            var userIdLogin = userLogin == null ? 0 : userLogin.Id;
            var video = await _videoService.FinVideoAsync((int)id);
           
            var video_Vm = new Video_vm();
            if (video != null)
            {

                var user = await _userManager.FindByIdAsync(video.AppUserId.ToString());
                var like = await _likeVideo.FindAsync(userIdLogin, video.Id);

                video_Vm.PosterImg = video.PosterImg;
                video_Vm.Name = video.Name;
                video_Vm.Id = video.Id;
                video_Vm.Reaction = like == null ? " " : like.Reaction;
                video_Vm.LinkVideo = video.LinkVideo;
                video_Vm.Avartar = user.Avartar;
                video_Vm.FirtsName = user.FirtsName;
                video_Vm.Like = video.Like;
                video_Vm.UserLike = like == null ? 0 : like.UserId;
                video_Vm.DisLike = video.DisLike;
                video_Vm.LastName = user.LastName;
                video_Vm.ViewCount = video.ViewCount;
                video_Vm.AppUserId = video.AppUserId;
                video_Vm.Description = video.Description;
                video_Vm.LoginExternal = user.LoginExternal;
                video_Vm.CreateDate = video.CreateDate;
                var lVideo = _videoService.GetAll().Where(x => x.CategorysId == video.CategorysId && x.Id != video.Id && x.HidenVideo).ToList();
                var lUser = _userService.GetAll();
                if (userLogin != null)
                {
                    if (CheckUserFollow(userLogin.Id, video_Vm.AppUserId))
                        userFollow = "true";
                }
                var comment = _commentService.GetAll().Where(x => x.VideoId == video.Id).ToList();
                ViewBag.VideoRelationShip = _videoService.GetVideo_Vm(lVideo, lUser).OrderByDescending(x => x.Id).ToPagedList(1, 8).ToList();
                ViewBag.Comment = _commentService.GetAll_vm(lUser, comment).OrderByDescending(x => x.Id).ToList();
                ViewBag.CheckUserFollow = userFollow;
                ViewBag.CountRegister = _channelService.GetAll().Where(x => x.ToUserId == video_Vm.AppUserId).Count();
            }
            return View(video_Vm);
        }
        public IActionResult VideoRelationship_Partial(int? page, int id)
        {
            if (page == null) page = 1;
            if (id != 0 && id > 0)
            {
                var video = _videoService.FinVideoAsync(id).Result;
                var lVideo = _videoService.GetAll().Where(x => x.CategorysId == video.CategorysId && x.Id != video.Id).ToList();
                var lUser = _userService.GetAll();
                var list_vm = _videoService.GetVideo_Vm(lVideo, lUser).OrderByDescending(x => x.Id).ToPagedList((int)page, 8);
                foreach (var item in list_vm)
                {
                    item.CreateDate = CaculatorHours.Caculator(item.CreateDate);
                }
                return View(list_vm);
            }
            return View(null);
        }
        [HttpPost]
        public async Task<IActionResult> CreateComment_Partial(string comments)
        {
            var comment = JsonConvert.DeserializeObject<CommentRequest>(comments);
            var result = await _commentService.Create(comment);
            if (result > 0)
            {
                var getComment = _commentService.GetCm_Vm(comment);//userid
                ViewBag.User = await _userService.FindUser(User.Identity.Name);
                return View(getComment);
            }
            return Content("Error");
        }


        [HttpPost]
        public async Task<IActionResult> FollowChannel(string data)
        {
            var flChannel = JsonConvert.DeserializeObject<FollowChannelRequest>(data);
            var result = await _channelService.Create(flChannel);
            var countFollow = _channelService.GetAll().Where(x => x.ToUserId == flChannel.ToUserId).Count();
            if (result > 0)
            {
                return Content(ConvertViewCount.ConvertView(countFollow).ToString());
            }
            return Content("Error");
        }
        [HttpPost]
        public async Task<IActionResult> CancelRegister(string data)
        {
            var flChannel = JsonConvert.DeserializeObject<FollowChannelRequest>(data);
            var result = await _channelService.Delete(flChannel.FromUserId, flChannel.ToUserId);
            var countFollow = _channelService.GetAll().Where(x => x.ToUserId == flChannel.ToUserId).Count();
            if (result > 0) return Content(ConvertViewCount.ConvertView(countFollow).ToString());
            return Content("Error");
        }
        public bool CheckUserFollow(int fromUserId, int toUserId)
        {
            var listFollow = _channelService.GetAll();
            foreach (var item in listFollow)
            {
                if (item.FromUserId == fromUserId && item.ToUserId == toUserId)
                    return true;
            }

            return false;
        }
        public async Task<IActionResult> Logout()
        {
            UserAuthenticated.LogOutUser(User.Identity.Name);
            await _signInManager.SignOutAsync();
            return Redirect("Index");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public async Task<IActionResult> Login(int? checkLock)
        {

            countLockout = 0;
            ViewBag.Titles = "Đăng nhập";
            //Cleare cookie external to sure clean cookie
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);


            ViewBag.ExternaLogin = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (checkLock == 1)
                ModelState.AddModelError(string.Empty, "Tài khoản bị khóa vui lòng liên hệ Admin!");
            if (checkLock == 2)
                ModelState.AddModelError(string.Empty, "Tài Khoản đã bị xóa!");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AppUserRequest model)
        {
            if (model.Email != userEmail)
            {
                countLockout = 0;
            }
            userEmail = model.Email;
            var user = await _userService.FindUser(model.Email);
            ViewBag.ExternaLogin = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản  chưa đăng ký, đăng ký để tiếp tục!");
                return View();
            }
            else
            {
                if (user.LockoutEnabled)
                {
                    var reusult = await _userService.Login(model);

                    if (reusult)
                    {
                        var login = new LoginRequest
                        {
                            Email = model.Email,
                            PasswordHash = model.PasswordHash
                        };
                        var token = await _userApiClient.Authenticated(login);
                        HttpContext.Session.SetString("Token", token);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        countLockout++;
                        if (countLockout >= 5 && user.LockoutEnabled)
                        {
                            await _userService.UpdateLockout(user);
                            ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa" +
                                " vui lòng liên hệ Admin để được hỗ trợ");
                            return View();
                        }
                        ModelState.AddModelError(string.Empty, "Lưu ý nhập sai 5 lần liên tiếp sẽ khóa tài khoản!");
                        ModelState.AddModelError(string.Empty, "Sai tài khoản hoặc mật khẩu vui lòng nhập lại!");
                        return View();

                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa" +
                        " vui lòng liên hệ Admin để được hỗ trợ");
                    return View();
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            var redirectUrl = Url.Action("ExternalCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalCallback()
        {
            string email = null;
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }
            if (info.LoginProvider == "Facebook")
                email = info.Principal.Claims.ToArray()[1].Value;
            else email = info.Principal.Claims.ToArray()[4].Value;
            // Sign in the user with this external login provider if the user already has a login.
            var users = await _userService.FindUser(email);

            if (users != null)
            {
                if (users.Status && users.LockoutEnabled)
                {
                    UserAuthenticated.checkUserAuthenticated(users);
                    await _signInManager.SignInAsync(users, isPersistent: false, info.LoginProvider);
                    return RedirectToAction("Index");
                }
                else
                {
                    if (users.LockoutEnabled == false)
                    {
                        return Redirect("Login/?checkLock=1");
                    }
                    if (users.Status == false)
                    {
                        return Redirect("Login/?checkLock=2");
                    }
                }


            }
            else
            {
                var user = _userService.SetAttributeUser(info).Result;
                var result1 = await _userManager.CreateAsync(user);
                if (result1.Succeeded)
                {

                    GenerationTokenEmail(user, ConfirmEmailAccount.External.ToString());
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Redirect("EmailVerification");
                }
            }
            return View();
        }
        public IActionResult Register()
        {
            ViewBag.Titles = "Đăng ký";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(AppUserRequest model, IFormFile avartarFile)
        {

            if (ModelState.IsValid)
            {
                if (_userService.Register(model, avartarFile).Result)
                {
                    var user = await _userService.FindUser(model.Email);
                    //generation token email
                    GenerationTokenEmail(user, ConfirmEmailAccount.Register.ToString());
                    return Redirect("EmailVerification");
                }
            }
            ModelState.AddModelError("Lỗi", "Đăng ký không thành công");
            return View();
        }
        public IActionResult EmailVerification() => View();
        public async void GenerationTokenEmail(AppUser user, string checkConirm)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var link = "";
            if (checkConirm.Contains("External"))
            {
                link = Url.Action("VerifyEmail", "Home", new { userId = user.Id, token, check = 1 }, Request.Scheme);
            }
            if (checkConirm.Contains("Register"))
            {
                link = Url.Action("VerifyEmail", "Home", new { userId = user.Id, token, check = 2 }, Request.Scheme);
            }
            if (checkConirm.Contains("ForgotPassword"))
            {
                link = Url.Action("VerifyEmail", "Home", new { userId = user.Id, token, check = 3 }, Request.Scheme);
            }
            _userService.SendEmail(user, link);
        }

        public async Task<IActionResult> VerifyEmail(string userId, string token, int check)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest();
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                if (check == 1 || check == 2)
                {
                    //Check user authenticated

                    UserAuthenticated.checkUserAuthenticated(user);
                    //--- end
                    ViewBag.Flag = true;
                    ViewBag.InfoUserLogin = user;
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    await _userManager.AddToRoleAsync(user, "User");
                    return View();
                }
                if (check == 3)
                {
                    ViewBag.Flag = false;
                    return View(user);
                }

            }
            return BadRequest();
        }
        public string SearchEmail(string email)
        {
            var user = _userManager.FindByEmailAsync(email);
            return JsonConvert.SerializeObject(user.Result);
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                GenerationTokenEmail(user, ConfirmEmailAccount.ForgotPassword.ToString());
                return Redirect("EmailVerification");
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(AppUser userRequest)
        {
            if (ModelState.IsValid)
            {
                if (await _userService.Update(userRequest))
                {
                    var user = await _userService.FindUser(userRequest.Email);
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    ViewBag.InfoUserLogin = user;
                    //Check user authenticated

                    UserAuthenticated.checkUserAuthenticated(user);
                    //----end
                    return Redirect("Index");
                }
            }
            ModelState.AddModelError("", "Cập nhật không thành công");
            ViewBag.Flag = false;
            return Redirect("VerifyEmail");
        }
    }
}
