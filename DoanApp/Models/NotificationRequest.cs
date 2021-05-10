using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class NotificationRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
       
        public int UserId { get; set; }
    }
}
