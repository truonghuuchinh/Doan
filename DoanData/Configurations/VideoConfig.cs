using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    public class VideoConfig : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder.ToTable("Video");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Status).HasDefaultValue<bool>(true);
            builder.Property(x => x.CreateDate).HasDefaultValue<string>("00/00/00 00:0:00");
            builder.HasOne(x => x.category).WithMany(x => x.VideoList).HasForeignKey(X => X.CategorysId);
            builder.HasOne(X => X.appUser).WithMany(x => x.Videos).HasForeignKey(x => x.AppUserId);
          
        }
    }
}
