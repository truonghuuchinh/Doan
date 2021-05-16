using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Models
{
   public class Notification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string CreateDate { get; set; }
        public int UserId { get; set; }
        public virtual AppUser appUser { get; set; }
    }
}
