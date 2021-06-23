using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class FollowChannelRequest
    {
        public int Id { get; set; }
        public bool Notifications { get; set; } = true;
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
    }
}
