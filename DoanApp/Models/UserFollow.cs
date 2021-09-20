using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class UserFollow
    {
        public string Email { get; set; }
        public List<AppUser> ListUser { get; set; }
    }
}
