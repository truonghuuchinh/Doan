using DoanApp.Areas.Administration.Models;
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

        List<UserAdmin> GetUserAdmin(string name);
        List<AppUser> GetChannel();
        void SendEmail(AppUser user,string link);
        Task<bool> Update(AppUser userRequest);
        Task<int> UpdateLockout(AppUser user);
        List<AppUser> GetAll();
        List<AppUser> GetUserFollow(string email);
        Task<int> UpdateImgChannel(int idUser,string ImgChannel);
        Task<AppUser> FindUserId(int id);
        Task<int> UpdateDescription(AppUserRequest request);
        Task<int> UpdateNameChannel(UpdateNameChannel request);
        Task<int> UpdateAvartar(int id,string avartar);
        Task<int> Delete(int id);
        Task<bool> Login(AppUserRequest model);
        Task<bool> Register(AppUserRequest model, IFormFile avartarFile);
        Task<AppUser> SetAttributeUser(ExternalLoginInfo info);
        Task<AppUser> FindUser(string email);
        Task<string> AuthenticatedApi(AppUserRequest request);

    }
}
