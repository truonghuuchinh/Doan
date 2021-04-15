using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    public class ReportVideoConfig : IEntityTypeConfiguration<ReportVideo>
    {
        public void Configure(EntityTypeBuilder<ReportVideo> builder)
        {
            builder.ToTable("ReportVideo");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreateDate).HasDefaultValue<string>("00/00/00 00:0:00");
            builder.HasOne(x => x.appUser).WithMany(x =>x.ReportVideos).HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.video).WithMany(x => x.ReportVideos).HasForeignKey(x => x.VideoId).OnDelete(DeleteBehavior.Restrict).HasPrincipalKey(t=>t.Id);
        }
    }
}
