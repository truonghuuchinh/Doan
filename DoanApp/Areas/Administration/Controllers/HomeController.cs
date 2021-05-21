using DoanApp.Commons;
using DoanApp.Models;
using DoanApp.Services;
using DoanData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
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
        static int countLockoutAdmin = 0;
        public HomeController(IUserService userService, 
            SignInManager<AppUser> signInManager, UserManager<AppUser> usermanager)
        {
            _userService = userService;
            _signInManager = signInManager;
            _userManager = usermanager;
        }
        
       
        public IActionResult Login()
        {
            countLockoutAdmin = 0;
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            
           await _signInManager.SignOutAsync();
            
            return RedirectToAction("Login","Home");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public string SearchEmail(string email)
        {
            var user = _userService.FindUser(email).Result;
            if(user!=null) return JsonConvert.SerializeObject(user);
            return "null";
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
                var url = Url.RouteUrl(new { action="EmailVerification",controller="Home",area="" });
                return Redirect(url);
            }
            return BadRequest();
        }
        public async void GenerationTokenEmail(AppUser user, string checkConirm)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
               var link = Url.Action("VerifyEmail", "Home", new { userId = user.Id, token}, Request.Scheme);
            _userService.SendEmail(user, link);
        }
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest();
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                    
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
                    var addUserAuthenticated = new UserAuthenticated();
                    addUserAuthenticated.checkUserAuthenticated(user);
                    //----end
                    return RedirectToAction("Index","User");
                }
            }
            ModelState.AddModelError("", "Cập nhật không thành công");
            ViewBag.Flag = false;
            return Redirect("VerifyEmail");
        }
        [HttpPost]
        public async Task<IActionResult> Login(AppUserRequest appUser,string RememberMe)
        {
            var result = await _userService.Login(appUser);
            var user = await _userService.FindUser(appUser.Email);
            if (result)
            {
                if (RememberMe!=null)
                {
                    var cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Append("userName", user.Email,cookieOptions);
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
                return RedirectToAction("Index","User");
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
                    if (countLockoutAdmin >= 5&user.LockoutEnabled)
                    {
                        await _userService.UpdatLockcout(user);
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
