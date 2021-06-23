using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Models
{
    public class VideoWatched
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual AppUser user { get; set; }
        public int VideoId { get; set; }
        public virtual Video video { get; set; }
    }
}
