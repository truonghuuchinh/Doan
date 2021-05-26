using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class Comment_vm
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int CommentId { get; set; }
        public string CreateDate { get; set; }
        public int Like { get; set; }
        public int DisLike { get; set; }
        public bool Status { get; set; } = true;
        public int UserId { get; set; }
        public bool LoginExternal { get; set; }
        public int VideoId { get; set; }
        public string ReplyFor { get; set; }
        public string Avartar { get; set; }
        public string FirtsName { get; set; }
        public string LastName { get; set; }

    }
}
