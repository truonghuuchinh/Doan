using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Models
{
    public class DetailPlayListVideo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public int VideoId { get; set; }
        public int PlayListId { get; set; }
        public bool Status { get; set; }
        public string PosterVideo { get; set; }
        public int CountItem { get; set; }
        public string CreateDate { get; set; }
    }
}
