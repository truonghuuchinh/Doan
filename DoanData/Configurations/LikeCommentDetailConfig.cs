using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
   public class LikeCommentDetailConfig: IEntityTypeConfiguration<LikeCommentDetail>
    {
        public void Configure(EntityTypeBuilder<LikeCommentDetail> builder)
        {
            builder.ToTable("LikeCommentDetail");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.user).WithMany(l => l.LikeComments).HasForeignKey(j => j.UserId);
            builder.HasOne(x => x.video).WithMany(x => x.LikeComments).HasForeignKey(x => x.VideoId).OnDelete(DeleteBehavior.NoAction).HasPrincipalKey(t => t.Id);
            builder.Property(x => x.Reaction).HasDefaultValue<string>("NoAction");
            
        }
    }
}
