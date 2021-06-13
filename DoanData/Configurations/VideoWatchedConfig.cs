using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    public class VideoWatchedConfig : IEntityTypeConfiguration<VideoWatched>
    {
        public void Configure(EntityTypeBuilder<VideoWatched> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.user).WithMany(x => x.VideoWatcheds).HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.video).WithMany(x => x.VideoWatcheds).HasForeignKey(x => x.VideoId).OnDelete(DeleteBehavior.NoAction).HasPrincipalKey(t => t.Id); ;
        }
    }
}
