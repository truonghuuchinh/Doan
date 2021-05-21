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

namespace DoanApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        static int countLockout = 0;
        public HomeController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IUserService userService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
          
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Popular()
        {
            return View();
        }
        public IActionResult DetailVideo()
        {
            return View();
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
