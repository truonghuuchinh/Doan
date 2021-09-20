using DoanApp.Commons;
using DoanApp.Models;
using DoanApp.ServiceApi;
using DoanApp.Services;
using DoanData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanApp.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IVideoService _videoService;
        private readonly IReportVideoService _reportService;
        private readonly ICategoryService _categoryService;
        private readonly IUserApiCient _userApiClient;

        static int countLockoutAdmin = 0;
        public HomeController(IUserService userService,
            SignInManager<AppUser> signInManager, UserManager<AppUser> usermanager,
            IVideoService service, IReportVideoService report, ICategoryService category,
            IUserApiCient userApiClient)
        {
            _userService = userService;
            _signInManager = signInManager;
            _userManager = usermanager;
            _videoService = service;
            _reportService = report;
            _categoryService = category;
            _userApiClient = userApiClient;
        }

        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Index()
        {
            ViewBag.Active = 0;
            ViewBag.TotalUser = _userService.GetAll().Count();
            ViewBag.TotalVideo = _videoService.GetAll().Count();
            ViewBag.TotalReport = _reportService.GetAll().Count();
            ViewBag.TotalCategory = _categoryService.GetAll().Result.Count;
            var listRankedVideo = _videoService.GetAll().OrderByDescending(x => x.ViewCount).Take(10).ToList();
            return View(listRankedVideo);
        }
        public string GetTotalReport()
        {
            var week = new GetWeek().GetIso8601WeekOfYear(DateTime.Now);
            var dateNow = DateTime.Now.ToString("MM-d-yyyy H-mm-ss").Split('-');
            var arrayDate = GetDateWeek.CaculatorDate(week, int.Parse(dateNow[1]), int.Parse(dateNow[0])).ToArray();
            var list = _reportService.GetAll().GroupBy(x => x.CreateDate.Split('-')[1]).Select(x => new
            {
                Name = x.Key,
                Count = x.Count()
            }).ToArray();

            for (int i = 0; i < arrayDate.Length; i++)
            {
                var flag = false;
                var tam = 0;
                for (int j = 0; j < list.Length; j++)
                {
                    if (arrayDate[i].Day == int.Parse(list[j].Name))
                    {
                        flag = true;
                        tam = (list[j].Count);
                    }
                }
                if (flag)
                    arrayDate[i].Day = tam;
                else arrayDate[i].Day = 0;
            }
            var lastItem = arrayDate[arrayDate.Length - 1];
            var arrayList = new ArrayList(arrayDate);
            arrayList.RemoveAt(arrayList.IndexOf(lastItem));
            arrayList.Insert(0, lastItem);
            return JsonConvert.SerializeObject(arrayList);
        }
        public string GetCreateDate()
        {

            var listVideo = _userService.GetAll().
                GroupBy(x => x.CreateDate.Split('-')[0]).Select(x => new
                {
                    Month = int.Parse(x.Key),
                    Count = x.Count()
                }).OrderBy(x => x.Month).ToList();
            return JsonConvert.SerializeObject(listVideo);
        }
        public string GetVideoCreate()
        {
            var listVideo = _videoService.GetAll().
                GroupBy(x => x.CreateDate.Split('-')[0]).Select(x => new
                {
                    Month = int.Parse(x.Key),
                    Count = x.Count()
                }).OrderBy(x => x.Month).ToList();
            return JsonConvert.SerializeObject(listVideo);
        }
        public string GetCategory()
        {

            var list = _videoService.GetAll().
                GroupBy(x => x.CategorysId).Select(x => new
                {
                    Id = x.Key,
                    Count = x.Count()
                });
            var listCategory = (from video in list
                                join cate in _categoryService.GetAll().Result on video.Id equals cate.Id
                                select new
                                {
                                    cate.Name,
                                    video.Count
                                }).ToList();
            var listNoneCategory = (from cate in _categoryService.GetAll().Result
                                    .Where(x => !_videoService.GetAll().Any(y => y.CategorysId == x.Id)).ToList()
                                    select new
                                    {
                                        cate.Name,
                                        Count = 0
                                    }).ToList();
            listCategory.AddRange(listNoneCategory);
            listCategory = listCategory.OrderByDescending(x => x.Count).ToList();
            return JsonConvert.SerializeObject(listCategory);
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated&&(User.IsInRole("Admin")|| User.IsInRole("Manager"))) {
                return Redirect("/Administration/Home/Index");
            }
            countLockoutAdmin = 0;
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(AppUserRequest model, IFormFile avartarFile)
        {

            if (ModelState.IsValid)
            {
                if (await _userService.Register(model, avartarFile))
                {
                    var user = await _userService.FindUser(model.Email);
                    //generation token email
                    GenerationTokenEmail(user, ConfirmEmailAccount.Register.ToString(), true);
                    var url = Url.RouteUrl(new { action = "EmailVerification", controller = "Home", area = "" });
                    return Redirect(url);
                }
            }
            ModelState.AddModelError("Lỗi", "Đăng ký không thành công");
            return View();
        }
        public async Task<IActionResult> Logout()
        {

            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Home");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public string SearchEmail(string email)
        {
            var user = _userService.FindUser(email).Result;
            if (user != null) return JsonConvert.SerializeObject(user);
            return "null";
        }

        [HttpPost]
        public IActionResult ConfirmPassword(string Email)
        {


            if (ModelState.IsValid)
            {
                var user = _userService.FindUser(Email).Result;
                GenerationTokenEmail(user, ConfirmEmailAccount.ForgotPassword.ToString(), false);
                var url = Url.RouteUrl(new { action = "EmailVerification", controller = "Home", area = "" });
                return Redirect(url);
            }
            return BadRequest();
        }

        public async void GenerationTokenEmail(AppUser user, string checkConirm, bool flag)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var link = Url.Action("VerifyEmail", "Home", new { userId = user.Id, token, flag }, Request.Scheme);
            _userService.SendEmail(user, link);
        }

        public async Task<IActionResult> VerifyEmail(string userId, string token, bool flag = false)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest();
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                if (flag)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    ViewBag.Flag = true;
                }
                return View(user);
            }
            return BadRequest();
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
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Cập nhật không thành công");
            ViewBag.Flag = false;
            return Redirect("VerifyEmail");
        }

        [HttpPost]
        public async Task<IActionResult> Login(AppUserRequest appUser, string RememberMe)
        {

            var result = await _userService.Login(appUser);

            var user = await _userService.FindUser(appUser.Email);

            if (result)
            {
                if (RememberMe != null)
                {
                    var cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Append("userName", user.Email, cookieOptions);
                    Response.Cookies.Append("password", appUser.PasswordHash, cookieOptions);
                }
                else
                {
                    foreach (var cookie in Request.Cookies.Keys)
                    {
                        Response.Cookies.Delete(cookie);
                    }
                }

                await _signInManager.SignInAsync(user, false);
                if (await _userManager.IsInRoleAsync(user, "Admin") || await _userManager.IsInRoleAsync(user, "Manager"))
                {
                    var loginRequest = new LoginRequest
                    {
                        Email=appUser.Email,
                        PasswordHash=appUser.PasswordHash
                    };
                    var token = await _userApiClient.Authenticated(loginRequest);
                    HttpContext.Session.SetString("Token", token);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "Tài khoản không có quyền vui lòng liên hệ Admin!");
                    return View();
                }
            }
            else
            {
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản không tồn tại!");
                    return View();
                }
                else
                {
                    countLockoutAdmin++;
                    if (countLockoutAdmin >= 5 & user.LockoutEnabled)
                    {
                        await _userService.UpdateLockout(user);
                        ModelState.AddModelError(string.Empty, "- Tài khoản đã bị khóa" +
                            " vui lòng liên hệ Admin để được hỗ trợ");
                        return View();
                    }
                    ModelState.AddModelError(string.Empty, "- Sai tài khoản hoặc mật khẩu vui lòng nhập lại!");
                    ModelState.AddModelError(string.Empty, "- Lưu ý nhập sai 5 lần liên tiếp sẽ khóa tài khoản!");
                    return View();
                }

            }
        }

    }
}