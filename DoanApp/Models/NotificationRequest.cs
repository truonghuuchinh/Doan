using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class NotificationRequest
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string CreateDate { get; set; } = DateTime.Now.ToString("d-mm-yyyy H:mm:ss");
        public string UserName { get; set; }
        public string AvartarUser { get; set; }
        public bool Watched { get; set; }
        public int FromUserId { get; set; }
        public bool LoginExternal { get; set; }
        public string PoterImg { get; set; }
        public bool Status { get; set; } = true;
        public int VideoId { get; set; }
        public int UserId { get; set; }
    }
}
