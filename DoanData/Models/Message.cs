using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public string CreateDate { get; set; }
        public string Avartar { get; set; }
        public bool LoginExternal { get; set; }
        public bool CheckWatched { get; set; } = false;
        public bool Watched { get; set; }
        public int SenderId { get; set; }
        public virtual AppUser AppusersSender { get; set; }
        public int ReceiverId { get; set; }
        public virtual AppUser AppusersReceiver { get; set; }
    }
}
