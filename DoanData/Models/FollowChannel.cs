using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Models
{
    public class FollowChannel
    {
        public int Id { get; set; }
        public int FromUserId { get; set; }
        public bool Notifications { get; set; }
        public virtual AppUser FromUser { get; set; }
        public int ToUserId { get; set; }
        public virtual AppUser ToUser { get; set; }
    }
}
