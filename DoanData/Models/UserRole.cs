using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Models
{
   public class UserRole
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public int UserId { get; set; }
        public virtual AppUser appUser { get; set; }
    }
}
