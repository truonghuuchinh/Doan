using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class CommentRequest
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int CommentId { get; set; }
        public string  Reaction { get; set; }
        public int Like { get; set; }
        public int DisLike { get; set; }
        public int UserId { get; set; }
        public string ReplyFor { get; set; }
        public int VideoId { get; set; }
    }
}
