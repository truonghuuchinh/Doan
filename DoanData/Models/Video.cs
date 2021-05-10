using System;
using System.Collections.Generic;

namespace DoanData.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LinkVideo { get; set; }
        public int Like { get; set; }
        public int DisLike { get; set; }
        public int ViewCount { get; set; }
        public string CreateDate { get; set; }
        public bool HidenVideo { get; set; }
        public bool Status { get; set; } = true;
        public int CategorysId { get; set; }
        public virtual Category category { get; set; }
        public int AppUserId { get; set; }
        public virtual AppUser appUser { get; set; }
     
        public List<Comment> Commmentsss { get; set; }
        public List<DetailVideo> DetailVideos { get; set; }
        public List<ReportVideo> ReportVideos { get; set; }
        public List<LikeVideoDetail> LikeVideoDetails { get; set; }
    }
}