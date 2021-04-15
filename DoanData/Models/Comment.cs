using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DoanData.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int CommentId { get; set; }
        public string CreateDate { get; set; }
        public int Like { get; set; }
        public int DisLike { get; set; }
        public  bool Status { get; set; }
        public int UserId { get; set; }
        public virtual AppUser appUser { get; set; }
        [ForeignKey("VideoId")]
        public int VideoId { get; set; }

        public virtual Video video { get; set; }
    }
}
