using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DoanData.Models
{

    public class AppUser:IdentityUser<int>
    {
        public string Address { get; set; }
        public string FirtsName { get; set; }
        public string LastName { get; set; }
        public string LastLogin { get; set; }
      
        public string Avartar { get; set; }
        public bool Status { get; set; }
        public List<Video> Videos { get; set; }
        public List<Comment> Comments { get; set; }
        public List<PlayList> PlayLists { get; set; }
        public List<FollowChannel> FromUsers { get; set; }
        public List<FollowChannel> ToUsers { get; set; }
        public List<LikeVideoDetail> LikeVideoDetails { get; set; }
        public List<ListVideoFavavorite> ListVideoFavavorites { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<ReportVideo> ReportVideos { get; set; }
    }
}
