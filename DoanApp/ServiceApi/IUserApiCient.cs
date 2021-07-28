using DoanApp.Areas.Administration.Models;
using DoanApp.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.ServiceApi
{
    public interface IUserApiCient
    {
        Task<string> Authenticated(LoginRequest request);
        Task<List<AppUser>> GetAll(string token,string nameSearch);
        Task<List<UserAdmin>> GetAllUserAdmin(string token, string emailUser, string nameSearch=null);
        Task<int> Delete(string token,int id);
        Task<string> RefreshGetToken(LoginRequest request);
        Task<string> CheckToken(string token, string email);
        
    }
}
