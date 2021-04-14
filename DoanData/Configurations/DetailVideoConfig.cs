using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    public class DetailVideoConfig : IEntityTypeConfiguration<DetailVideo>
    {
       
        public void Configure(EntityTypeBuilder<DetailVideo> builder)
        {
            builder.ToTable("DetailVideo");
            builder.HasKey(X => X.Id);
            builder.HasOne(x => x.playList).WithMany(l => l.Details).HasForeignKey(p => p.PlayListId).HasPrincipalKey(t=>t.Id);
            builder.HasOne(x => x.video).WithMany(l => l.DetailVideos).HasForeignKey(p => p.VideoId).OnDelete(DeleteBehavior.NoAction).HasPrincipalKey(t=>t.Id);
           
            builder.Property(x => x.Status).HasDefaultValue<bool>(true);
        }
    }
}
