using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Models
{
   public class Notification
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public string CreateDate { get; set; }
        public string AvartarUser { get; set; }
        public bool LoginExternal { get; set; }
        public string PoterImg { get; set; }
        public bool Watched { get; set; }
        public bool Status { get; set; }
        public int VideoId { get; set; }
        public virtual Video video { get; set; }
        public int FromUserId { get; set; }
        public virtual AppUser fromAppUser { get; set; }
        public int UserId { get; set; }
        public virtual AppUser appUser { get; set; }
    }
}
