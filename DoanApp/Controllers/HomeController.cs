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

namespace DoanApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IVideoService _videoService;
        private readonly ICommentService _commentService;
        static int countLockout = 0;
        public HomeController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IUserService userService,IVideoService videoService,
            ICommentService commentService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _videoService = videoService;
            _userService = userService;
            _commentService = commentService;
        }
        public IActionResult Index(int? page)
        {
            if (page == null) page = 1;
            var pageSize = 20;
            var pageNumber = page ?? 1;
            List<Video_vm> listVideo_Vm = new List<Video_vm>();
            var listVideo = _videoService.GetAll();
            var listUser = _userService.GetAll();
            listVideo_Vm = GetVideo_Vm(listVideo, listUser);
            return View(listVideo_Vm.ToPagedList(pageNumber,pageSize));
        }
        public List<Video_vm> GetVideo_Vm(List<Video> lVideo,List<AppUser> lUser)
        {
            List<Video_vm> listVideo_Vm = new List<Video_vm>();
            var listVideo = (from video in lVideo
                             join user in lUser on video.AppUserId equals user.Id
                             select new
                             {
                                 video,
                                 user
                             });

            foreach (var item in listVideo)
            {
                var video = new Video_vm();
                video.PosterImg = item.video.PosterImg;
                video.Name = item.video.Name;
                video.Id = item.video.Id;
                video.LinkVideo = item.video.LinkVideo;
                video.Avartar = item.user.Avartar;
                video.FirtsName = item.user.FirtsName;
                video.LastName = item.user.LastName;
                video.AppUserId = item.user.Id;
                video.ViewCount = item.video.ViewCount;
                video.LoginExternal = item.user.LoginExternal;
                video.CreateDate = item.video.CreateDate;
                listVideo_Vm.Add(video);
            }
            return listVideo_Vm;

        }
        public IActionResult Popular()
        {
            return View();
        }
        public async Task<IActionResult> DetailVideo(int? id)
        {
            var video =await  _videoService.FinVideoAsync((int)id);
            var user = await _userManager.FindByIdAsync(video.AppUserId.ToString());
            var video_Vm = new Video_vm();
            video_Vm.PosterImg = video.PosterImg;
            video_Vm.Name = video.Name;
            video_Vm.Id = video.Id;
            video_Vm.LinkVideo = video.LinkVideo;
            video_Vm.Avartar = user.Avartar;
            video_Vm.FirtsName = user.FirtsName;
            video_Vm.LastName = user.LastName;
            video_Vm.ViewCount = video.ViewCount;
            video_Vm.AppUserId = video.AppUserId;
            video_Vm.LoginExternal = user.LoginExternal;
            video_Vm.CreateDate = video.CreateDate;
            var lVideo = _videoService.GetAll().Where(x => x.CategorysId == video.CategorysId&&x.Id!=video.Id).ToList();
            var lUser = _userService.GetAll();
            ViewBag.VideoRelated = GetVideo_Vm(lVideo, lUser).ToPagedList(1, 15).ToList();
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserLogin = UserAuthenticated.GetUser(User.Identity.Name);
            }
            var comment = _commentService.GetAll().Where(x => x.VideoId == video.Id).ToList();
            ViewBag.Comment = _commentService.GetAll_vm(lUser, comment).OrderByDescending(x => x.Id).ToList();
;            return View(video_Vm);
        }
        [HttpPost]
        public async Task<IActionResult> CreateComment_Partial(string comments)
        {
            var comment = JsonConvert.DeserializeObject<CommentRequest>(comments);
            var result = await _commentService.Create(comment);
            if(result>0)
            {
                var getComment = _commentService.GetCm_Vm(comment);
                ViewBag.User = await _userService.FindUser(User.Identity.Name);
                return View(getComment);
            }
            return Content("Error");
        }
        public IActionResult FavoritedVideo()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("Index");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public async Task<IActionResult> Login()
        {
            countLockout = 0;
            ViewBag.Titles = "Đăng nhập";
            //Cleare cookie external to sure clean cookie
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewBag.ExternaLogin = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AppUserRequest model)
        {
           
         
            var reusult = await _userService.Login(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
                if (reusult)
                {
                          return RedirectToAction("Index", "Home");
                 }
                else
                {
               
                    ViewBag.ExternaLogin = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Tài khoản  chưa đăng ký, đăng ký để tiếp tục!");
                        return View();
                    }
                    else
                    {
                        countLockout++;
                        if (countLockout >= 5&&user.LockoutEnabled)
                        {
                            await _userService.UpdatLockcout(user);
                            ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa" +
                                " vui lòng liên hệ Admin để được hỗ trợ");
                            return View();
                        }
                        ModelState.AddModelError(string.Empty, "Lưu ý nhập sai 5 lần liên tiếp sẽ khóa tài khoản!");
                        ModelState.AddModelError(string.Empty, "Sai tài khoản hoặc mật khẩu vui lòng nhập lại!");
                        return View();
                    }
                    
                }
        }

        [HttpPost]
        public  IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
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
                email = info.Principal.Claims.ToArray()[2].Value;
            else email = info.Principal.Claims.ToArray()[4].Value;
            // Sign in the user with this external login provider if the user already has a login.
            var users = await _userService.FindUser(email);

            if (users!=null)
            {
                
                var addUserAuthenticated = new UserAuthenticated();
                addUserAuthenticated.checkUserAuthenticated(users);
                await _signInManager.SignInAsync(users, isPersistent: false, info.LoginProvider);
                return RedirectToAction("Index");
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
        public async Task<IActionResult> Register(AppUserRequest model,IFormFile avartarFile)
        {
            
            if (ModelState.IsValid)
            {
                    if (_userService.Register(model,avartarFile).Result)
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
                    var addUserAuthenticated = new UserAuthenticated();
                    addUserAuthenticated.checkUserAuthenticated(user);
                    //--- end
                    ViewBag.Flag = true;
                    ViewBag.InfoUserLogin = user;
                    await _signInManager.SignInAsync(user, isPersistent: false);
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
        public IActionResult ConfirmPassword(AppUser userRequest)
        {
            var user = new AppUser();
           
            if (ModelState.IsValid)
            {
                user.Id = userRequest.Id;
                user.Avartar = userRequest.Avartar;
                user.LoginExternal = userRequest.LoginExternal;
                user.Email = userRequest.Email;
                user.SecurityStamp = userRequest.SecurityStamp;
                GenerationTokenEmail(user, ConfirmEmailAccount.ForgotPassword.ToString());
                return Redirect("EmailVerification");
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(AppUser userRequest)
        {
            if (ModelState.IsValid)
            {
                    if(await _userService.Update(userRequest))
                    {
                    var user =await _userService.FindUser(userRequest.Email);
                        await _signInManager.SignInAsync(user, isPersistent: true);
                        ViewBag.InfoUserLogin = user;
                        //Check user authenticated
                        var addUserAuthenticated = new UserAuthenticated();
                        addUserAuthenticated.checkUserAuthenticated(user);
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
