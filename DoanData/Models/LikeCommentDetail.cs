using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DoanData.Models
{
    public class LikeCommentDetail
    {
        public int Id { get; set; }
        public string Reaction { get; set; }
        public int Comment { get; set; }
        public int UserId { get; set; }
        public virtual AppUser user { get; set; }
        public int VideoId { get; set; }
        public virtual Video video { get; set; }
    }
}
