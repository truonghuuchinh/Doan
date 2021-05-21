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
        public  void checkUserAuthenticated(AppUser user)
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
    }
}
