using DoanApp.Commons;
using DoanApp.Models;
using DoanData.DoanContext;
using DoanData.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly DpContext _context;
        public UserService(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, DpContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<AppUser> FindUser(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public  List<AppUser> GetAll()
        {
            return  _context.AppUser.ToList();
        }

        public List<AppUser> GetUserFollow(string email)
        {
            var userLogin = UserAuthenticated.GetUser(email);
            var listFollow = _context.FollowChannel.Where(x => x.FromUserId == userLogin.Id);
            var lisUser = new List<AppUser>();
            lisUser = (from user in _context.AppUser
                       join follow in listFollow on user.Id equals follow.ToUserId
                       select user).ToList();
            return lisUser;
        }

        public async Task<bool> Login(AppUserRequest model)
        {
           
            var user = await FindUser(model.Email);
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.PasswordHash, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {

                UserAuthenticated.checkUserAuthenticated(user);
                 return result.Succeeded;
            }
            return false;
        }

        public async Task<bool> Register(AppUserRequest model, IFormFile avartarFile)
        {
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirtsName = model.FirtsName,
                LastName = model.LastName,
                LockoutEnabled=false
            };
            var result = await _userManager.CreateAsync(user, model.PasswordHash);
            if (result.Succeeded)
            {
                var findUser = await _userManager.FindByEmailAsync(model.Email);
                if (avartarFile == null) findUser.Avartar = "avartarDefault.JPG";
                else
                {
                    findUser.Avartar = findUser.Id.ToString() + "." + avartarFile.FileName.Split('.')[1];
                    using (var fileStream = new FileStream(Path.Combine("wwwroot" + "/Client/avartar", findUser.Avartar),
                        FileMode.Create, FileAccess.Write))
                    {
                        avartarFile.CopyTo(fileStream);
                    }
                }
                var resultUpdate = await _userManager.UpdateAsync(findUser);
                if (resultUpdate.Succeeded) return true;
            }
            return false;
        }

        public void SendEmail(AppUser user,string link)
        {

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
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("khleson79929@gmail.com", "phlakkmxjeceukbu");
                client.Send(message);
                client.Disconnect(true);
            }
        }

        public async  Task<AppUser> SetAttributeUser(ExternalLoginInfo info)
        {
            var userLogin = new AppUser();
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                userLogin.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            if (info.Principal.HasClaim(c => c.Type == "picture"))
                userLogin.Avartar = info.Principal.FindFirstValue("picture");
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
            var user = new AppUser
            {
                UserName = userLogin.Email,
                Email = userLogin.Email,
                FirtsName = userLogin.FirtsName,
                LastName = userLogin.LastName,
                Avartar = userLogin.Avartar,
                LoginExternal = true
            };
            return user;
        }

        public async Task<bool> Update(AppUser userRequest)
        {
            var user = await _userManager.FindByEmailAsync(userRequest.Email);
            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (removeResult.Succeeded)
            {
                var resultUpdate = await _userManager.AddPasswordAsync(user, userRequest.PasswordHash);
                if (resultUpdate.Succeeded)
                {
                    return true;
                }
            }
            return false;
         }

        public async Task<int> UpdatLockcout(AppUser user)
        {
            var users = _context.Users.FirstOrDefault(x => x.Id == user.Id);
            user.LockoutEnabled = true;
            _context.Update(users);
            return await _context.SaveChangesAsync();
        }
    }
}
