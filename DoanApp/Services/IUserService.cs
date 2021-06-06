using DoanApp.Models;
using DoanData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public interface IUserService
    {
        Task<bool> Login(AppUserRequest model);
        Task<bool> Register(AppUserRequest model,IFormFile avartarFile);
        Task<AppUser> SetAttributeUser(ExternalLoginInfo info);
        Task<AppUser> FindUser(string email);
        void SendEmail(AppUser user,string link);
        Task<bool> Update(AppUser userRequest);
        Task<int> UpdatLockcout(AppUser user);
        List<AppUser> GetAll();
        List<AppUser> GetUserFollow(string email);
    }
}
