using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Models
{
   public class UserRole
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string CreateDate { get; set; }
        public int UserId { get; set; }
        public virtual AppUser appUser { get; set; }
        public int ActionId { get; set; }
        public  virtual Action action { get; set; }


    }
}
