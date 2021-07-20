using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class Message_Vm
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public string Avartar { get; set; }
        public bool Watched { get; set; } = true;
        public bool LoginExternal { get; set; }
        public bool Online { get; set; } = false;
        public string CreateDate { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public bool CheckWatched { get; set; }
    }
}
