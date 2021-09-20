using DoanApp.Areas.Administration.Models;
using DoanApp.Commons;
using DoanApp.Models;
using DoanData.DoanContext;
using DoanData.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly DpContext _context;
        private readonly IConfiguration _config;
        public UserService(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, DpContext context,RoleManager<AppRole> role,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = role;
            _config = config;
        }

        public async Task<string> AuthenticatedApi(AppUserRequest request)
        {
            var user =await _userManager.FindByEmailAsync(request.Email);
            if (user== null) return "-1";
            //var result =await _signInManager.PasswordSignInAsync(request.Email, request.PasswordHash, request.RememberMe, true);
            var role =await  _userManager.GetRolesAsync(user);
            if (user==null) return "0";
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name,user.FirtsName),
                new Claim(ClaimTypes.Role,string.Join(';',role)),
                new Claim(ClaimTypes.Name,user.FirtsName+" "+user.LastName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Tokens:Issuer"], _config["Tokens:Issuer"],
                claims, expires: DateTime.Now.AddHours(1), signingCredentials: credential);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<int> Delete(int id)
        {
            var user = _context.AppUser.Where(x => x.Status).FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                user.Status = false;
                _context.Update(user);
                return await  _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<AppUser> FindUser(string email)
        {
            var user = await _context.AppUser.FirstOrDefaultAsync(x => x.Email == email);
            return user; 
        }

        public async Task<AppUser> FindUserId(int id)
        {
            return await _context.AppUser.FindAsync(id);
        }

        public  List<AppUser> GetAll()
        {
            return  _context.AppUser.Where(x=>x.Status).ToList();
        }

        public List<AppUser> GetChannel()
        {
            var listVideo = _context.Video.Where(x => x.Status && x.HidenVideo).GroupBy(x => x.AppUserId).Select(x => new
            {
                x.Key
            });
            var lisUser = new List<AppUser>();
            lisUser = (from user in GetAll().ToList()
                       join follow in listVideo on user.Id equals follow.Key
                       select user).ToList();
            return lisUser;
        }

        public List<UserAdmin> GetUserAdmin(string name)
        {
            var video = _context.Video.GroupBy(x => x.AppUserId).Select(y => new
            {
                Key = y.Key,
                TotalVideo = y.Count(),
                TotalView = y.Sum(x => x.ViewCount)
            });
            var userNovideo = GetAll().Where(x => !_context.Video.Any(y => y.AppUserId == x.Id)&&x.UserName!=name).ToList();
            var userAdmin = (from user in GetAll().Where(x=>x.UserName!=name).ToList()
                             join vd in video on user.Id equals vd.Key
                             select new
                             {
                                 Id=user.Id,
                                 Name=user.FirtsName+" "+user.LastName,
                                 CreateDate=user.CreateDate,
                                 TotalVideo=vd.TotalVideo,
                                 TotalView=vd.TotalView,
                                 LoginExternal=user.LoginExternal,
                                 Avartar=user.Avartar,
                                 LockOutEnabled=user.LockoutEnabled
                             }).ToList();
            var listUserAdmin = new List<UserAdmin>();
            var listNoUserAdmin = new List<UserAdmin>();
            foreach (var item in userAdmin)
            {
                var us = new UserAdmin();
                us.Id = item.Id;
                us.Name = item.Name;
                us.CreateDate = item.CreateDate;
                us.TotalVideo = item.TotalVideo;
                us.TotalView = item.TotalView;
                us.LoginExternal = item.LoginExternal;
                us.Avartar = item.Avartar;
                us.LockOutEnabled = item.LockOutEnabled;
                listUserAdmin.Add(us);
            }
            foreach (var item in userNovideo)
            {
                var us = new UserAdmin();
                us.Id = item.Id;
                us.Name = item.FirtsName+" "+item.LastName;
                us.CreateDate = item.CreateDate;
                us.TotalVideo = 0;
                us.TotalView = 0;
                us.LockOutEnabled = item.LockoutEnabled;
                us.LoginExternal = item.LoginExternal;
                us.Avartar = item.Avartar;
                listNoUserAdmin.Add(us);
            }
            listUserAdmin.AddRange(listNoUserAdmin);
            listUserAdmin = listUserAdmin.OrderByDescending(x => x.Id).ToList();
            return listUserAdmin;
        }

        public List<AppUser> GetUserFollow(string email)
        {
            var userLogin = UserAuthenticated.GetUser(email);
            var listFollow = _context.FollowChannel.Where(x => x.FromUserId == userLogin.Id);
            var lisUser = new List<AppUser>();
            lisUser = (from user in GetAll().ToList()
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
                LockoutEnabled = false,
                CreateDate = new GetDateNow().DateNow
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

        public async void SendEmail(AppUser user,string link)
        {

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("QUANTRIHETHONG", "khleson79929@gmail.com"));
            message.To.Add(new MailboxAddress("XACNHANEMAIL", user.Email));
            message.Subject = "Confirm Email";

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "<div>" +
               
                "<h4 style=\" margin-top:25px; margin-left:10px;\">Tài khoản của bạn đã được tạo thành công vui lòng xác nhận Email để hoàn thành việc đăng ký</h3>" +
                "<div style=\"background: #1da1f2;width: 150px;height: 86px;margin: 11px;margin-left: 140px;border-radius: 10px;padding-top: 63px;padding-left: 40px;\">" +
                "<a style=\"font-size:16px; text-decoration:none; color:white;\" href=\"" + link + "\">Xác nhận email</a>" +
                "</div>" +
                "</div>"
            };
            using (var client = new SmtpClient())
            {
                //587 hoặc 465
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Timeout = 100000;
                client.Authenticate("khleson79929@gmail.com", "phlakkmxjeceukbu");
                await client.SendAsync(message);
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
                userLogin.LastName = info.Principal.Claims.ToArray()[3].Value;
                userLogin.FirtsName = info.Principal.Claims.ToArray()[4].Value;
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
                LoginExternal = true,
                CreateDate=new GetDateNow().DateNow
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

        public async Task<int> UpdateAvartar(int id,string avartar)
        {
            var user = await FindUserId(id);
            if(user!=null)
            {
                user.Avartar = avartar;
                user.LoginExternal = false;
                _context.Update(user);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> UpdateDescription(AppUserRequest request)
        {
            var user = await FindUserId(request.Id);
            if (user != null)
            {
                user.DescriptionChannel = request.DescriptionChannel;
                _context.Update(user);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> UpdateImgChannel(int idUser,string ImgChannel)
        {
            var user = _context.AppUser.FirstOrDefault(X => X.Id == idUser);
            if (user != null&&ImgChannel!=null)
            {
                user.ImgChannel = ImgChannel;
                _context.Update(user);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> UpdateLockout(AppUser user)
        {
            var users = _context.Users.FirstOrDefault(x => x.Id == user.Id);
            if (users != null)
            {
                user.LockoutEnabled = user.LockoutEnabled ? false : true;
                _context.Update(users);
                return await _context.SaveChangesAsync();
            }
            return -1;
        }
    }
}
