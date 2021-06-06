using DoanApp.Models;
using DoanApp.Services;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Commons
{
    public class UserAuthenticated
    {
       
        public static List<AppUser> ListtUser = new List<AppUser>();
        public static List<UserFollow> ListUserFollow = new List<UserFollow>();
        public static void checkUserAuthenticated(AppUser user)
        {
            var count = 0;
            if (ListtUser.Count> 0)
            {
                foreach (var item in ListtUser)
                {
                    if (item.Email == user.Email) count++;
                }
                if (count == 0)
                    ListtUser.Add(user);
            }
            else
            {
                ListtUser.Add(user);
            }
           
        }
        public static AppUser GetUser(string email)
        {
            foreach (var item in ListtUser)
            {
                if (item.Email ==email)
                {
                    return item;
                }
            }
            return null;
        }
        public static  void SetUserFollow(List<AppUser> listfollow,string email)
        {
            var itemUser = new UserFollow();
            if (ListUserFollow.Count == 0)
            {
                itemUser.Email = email;
                itemUser.ListUser = listfollow;
                ListUserFollow.Add(itemUser);
            }
            else
            {
                foreach (var item in ListUserFollow)
                {
                    if (item.Email != email)
                    {
                        itemUser.Email = email;
                        itemUser.ListUser = listfollow;
                        ListUserFollow.Add(itemUser);
                        break;
                    }
                }
            }
            
            
        }
        public static List<AppUser> GetUserFollow(string email)
        {
            foreach (var item in ListUserFollow)
            {
                if (item.Email == email) return item.ListUser;
            }
            return null;
        }
        
    }
}
