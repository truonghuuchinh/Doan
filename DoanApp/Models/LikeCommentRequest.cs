using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class LikeCommentRequest
    {
        public int Id { get; set; }
        public int IdComment { get; set; }
        public  string Reaction { get; set; }
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public int VideoId { get; set; }
    }
}
