using DoanData.Configurations;
using DoanData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.DoanContext
{
    public class DpContext : IdentityDbContext<AppUser, AppRole, int>
    {

        public DpContext(DbContextOptions<DpContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new AppRoleConfig());
            builder.ApplyConfiguration(new AppuserConfig());
            builder.ApplyConfiguration(new CategoryConfig());
            builder.ApplyConfiguration(new CommentConfig());
            builder.ApplyConfiguration(new DetailVideoConfig());
            builder.ApplyConfiguration(new FollowChannelConfig());
            builder.ApplyConfiguration(new LikeVideoDetailConfig());
            builder.ApplyConfiguration(new ListVideoFavoriteConfig());
            builder.ApplyConfiguration(new NotificationConfig());
            builder.ApplyConfiguration(new PlaylistConfig());
            builder.ApplyConfiguration(new ReportVideoConfig());
            builder.ApplyConfiguration(new VideoConfig());
            builder.ApplyConfiguration(new LikeCommentDetailConfig());
            builder.ApplyConfiguration(new VideoWatchedConfig());

        }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<DetailVideo> DetailVideo { get; set; }
        public DbSet<FollowChannel> FollowChannel { get; set; }
        public DbSet<LikeVideoDetail> LikeVideoDetail { get; set; }
        public DbSet<ListVideoFavavorite> ListVideoFavavorite { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<PlayList> PlayList { get; set; }
        public DbSet<ReportVideo> ReportVideo { get; set; }
        public DbSet<Video> Video { get; set; }
        public DbSet<VideoWatched> VideoWatched { get; set; }
        public DbSet<LikeCommentDetail> LikeComments {get;set;}
    }
}
