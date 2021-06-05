using DoanData.Commons;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Models
{
    public class LikeVideoDetail
    {
        public int Id { get; set; }
        public string Reaction { get; set; } = "NoAction";
        public int VideoId { get; set; }
        public virtual Video video {get;set;}
        public int UserId { get; set; }
        public virtual AppUser appuser { get; set; }
    }
}
