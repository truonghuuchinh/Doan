using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class VideoWatchedRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VideoId { get; set; }
    }
}
