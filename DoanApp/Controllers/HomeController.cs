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
        private IUserRoleService _roleService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly DpContext _context;
        public HomeController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, DpContext context,
            IUserRoleService roleService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleService = roleService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Popular()
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
            ViewBag.Titles = "Đăng nhập";
            //Cleare cookie external to sure clean cookie
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewBag.ExternaLogin = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AppUserRequest model)
        {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.PasswordHash, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {

                 var user = await _userManager.FindByEmailAsync(model.Email);
                    UserAuthenticated.Email = user.Email;
                    UserAuthenticated.LoginExternal = user.LoginExternal;
                    UserAuthenticated.FirtsName = user.FirtsName;
                    UserAuthenticated.LastName = user.LastName;
                    UserAuthenticated.Avartar = user.Avartar;
                     return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ExternaLogin = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                    ModelState.AddModelError(string.Empty, "Sai tài khoản hoặc mật khẩu vui lòng nhập lại!");
                    return View();
                }
        }
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
             HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            var redirectUrl = Url.Action("ExternalCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }
            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var userLogin = new AppUser();
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    userLogin.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
                }
                if(info.Principal.HasClaim(c=>c.Type== "picture"))
                    userLogin.Avartar= info.Principal.FindFirstValue("picture");
                if (info.LoginProvider == "Facebook")
                {
                    userLogin.LastName = info.Principal.Claims.ToArray()[4].Value;
                    userLogin.FirtsName = info.Principal.Claims.ToArray()[5].Value;
                }
                else
                {
                    userLogin.LastName = info.Principal.Claims.ToArray()[2].Value;
                    userLogin.FirtsName = info.Principal.Claims.ToArray()[3].Value;
                }
                var user = new AppUser { UserName = userLogin.Email, 
                    Email = userLogin.Email,FirtsName=userLogin.FirtsName,LastName=userLogin.LastName,
                Avartar=userLogin.Avartar,LoginExternal=true};
                var result1 = await _userManager.CreateAsync(user);
                if (result1.Succeeded)
                {
                    var result2 = await _userManager.AddLoginAsync(user, info);
                    if (result2.Succeeded)
                    {
                        GenerationTokenEmail(user, ConfirmEmailAccount.External.ToString());
                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return Redirect("EmailVerification");
                    }
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
                var user = new AppUser { UserName = model.Email, 
                    Email = model.Email,FirtsName=model.FirtsName,LastName=model.LastName};
                var result = await _userManager.CreateAsync(user, model.PasswordHash);
                if (result.Succeeded)
                {
                    var findUser =await  _userManager.FindByEmailAsync(model.Email);
                    if (avartarFile == null) findUser.Avartar = "avartarDefault.png";
                    else
                    {
                        findUser.Avartar = findUser.Id.ToString() + "." + avartarFile.FileName.Split('.')[1];
                        using (var fileStream = new FileStream(Path.Combine("wwwroot" + "/Client/avartar", findUser.Avartar),
                            FileMode.Create, FileAccess.Write))
                        {
                            avartarFile.CopyTo(fileStream);
                        }
                    }
                    var resultUpdate=await _userManager.UpdateAsync(findUser);
                    if (resultUpdate.Succeeded)
                    {
                        //generation token email
                        GenerationTokenEmail(user, ConfirmEmailAccount.Register.ToString());
                        return Redirect("EmailVerification");
                    }
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
                link = Url.Action(nameof(VerifyEmail), "Home", new { userId = user.Id, token, check = 1 }, Request.Scheme);
            }
            if (checkConirm.Contains("Register"))
            {
                link = Url.Action(nameof(VerifyEmail), "Home", new { userId = user.Id, token, check = 2 }, Request.Scheme);
            }
            if (checkConirm.Contains("ForgotPassword"))
            {
                link = Url.Action(nameof(VerifyEmail), "Home", new { userId = user.Id, token, check = 3 }, Request.Scheme);
            }
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Confirm Email", "khleson79929@gmail.com"));
            message.To.Add(new MailboxAddress("test", user.Email));
            message.Subject = "Confirm Email Register";

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "<a href=\"" + link + "\">Please click to confirm email</a>"
            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("khleson79929@gmail.com", "phlakkmxjeceukbu");
                client.Send(message);
                client.Disconnect(true);
            }

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
                    var roleRequest = new UserRoleRequest("User",user.Id);
                    await _roleService.Create(roleRequest);
                    ViewBag.Flag = true;
                    await _signInManager.SignInAsync(user, isPersistent: true);
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
        public IActionResult ConfirmPassword(AppUserRequest userRequest)
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
        public async Task<IActionResult> UpdatePassword(AppUserRequest userRequest)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(userRequest.Email);
                var removeResult = await _userManager.RemovePasswordAsync(user);
                if (removeResult.Succeeded)
                {
                    var resultUpdate = await _userManager.AddPasswordAsync(user,userRequest.PasswordHash);
                    if (resultUpdate.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: true);
                        return Redirect("Index");
                    }
                }
            }
            ModelState.AddModelError("", "Cập nhật không thành công");
            ViewBag.Flag = false;
            return Redirect("VerifyEmail");
        }
    }
}
