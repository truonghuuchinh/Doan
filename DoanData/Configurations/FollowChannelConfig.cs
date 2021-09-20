using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    class FollowChannelConfig : IEntityTypeConfiguration<FollowChannel>
    {
        public void Configure(EntityTypeBuilder<FollowChannel> builder)
        {
            builder.ToTable("FollowChannel");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.FromUser).WithMany(j => j.FromUsers).HasForeignKey(l => l.FromUserId).HasPrincipalKey(t=>t.Id);
            builder.HasOne(x => x.ToUser).WithMany(j => j.ToUsers).HasForeignKey(l => l.ToUserId).OnDelete(DeleteBehavior.NoAction).HasPrincipalKey(t=>t.Id);
        }
    }
}
