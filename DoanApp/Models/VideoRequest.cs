using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class VideoRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LinkVideo { get; set; }
        public int Like { get; set; }
        public int DisLike { get; set; }
        public int ViewCount { get; set; }
        public bool HidenVideo { get; set; }
        public int CategorysId { get; set; }
        public int AppUserId { get; set; }
    }
}
