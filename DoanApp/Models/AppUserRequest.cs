using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class AppUserRequest
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public bool EmailConfirmed { get; set; }
        public string UserName { get; set; }
        public bool LockoutEnabled { get; set; }
        public string Address { get; set; }
        public string FirtsName { get; set; }
        public string LastName { get; set; }
        public string LastLogin { get; set; }
        public string Avartar { get; set; }
    }
}
