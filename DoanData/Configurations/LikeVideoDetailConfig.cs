using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    public class LikeVideoDetailConfig : IEntityTypeConfiguration<LikeVideoDetail>
    {
        public void Configure(EntityTypeBuilder<LikeVideoDetail> builder)
        {
            builder.ToTable("LikeVideoDetail");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.appuser).WithMany(l => l.LikeVideoDetails).HasForeignKey(j => j.UserId);
            builder.HasOne(x => x.video).WithMany(x => x.LikeVideoDetails).HasForeignKey(x => x.VideoId).OnDelete(DeleteBehavior.NoAction).HasPrincipalKey(t => t.Id);
            builder.Property(x => x.CreateDate).HasDefaultValue<string>("00/00/00 00:00:00");

        }
    }
}
