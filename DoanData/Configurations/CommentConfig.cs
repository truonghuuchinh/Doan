using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    public class CommentConfig : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comment");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.appUser).WithMany(s => s.Comments).HasForeignKey(l => l.UserId);
            builder.HasOne(x => x.video).WithMany(s => s.Commmentsss).HasForeignKey(l => l.VideoId).OnDelete(DeleteBehavior.Restrict).HasPrincipalKey(t => t.Id);
            builder.Property(x => x.CreateDate).HasDefaultValue<string>("00/00/00 00:0:00");
            builder.Property(x => x.Status).HasDefaultValue<bool>(true);
        }
    }
}
