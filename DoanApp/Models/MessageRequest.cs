using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class MessageRequest
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public string Avartar { get; set; }
        public bool Watched { get; set; } = false;
        public bool LoginExternal { get; set; }

        public string CreateDate { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
    }
}
