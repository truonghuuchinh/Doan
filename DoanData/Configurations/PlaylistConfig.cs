using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    public class PlaylistConfig : IEntityTypeConfiguration<PlayList>
    {
        public void Configure(EntityTypeBuilder<PlayList> builder)
        {

            builder.ToTable("PlayList");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreateDate).HasDefaultValue<string>("00/00/00 00:0:00");
            builder.Property(x => x.Status).HasDefaultValue<bool>(true);
            builder.HasOne(x => x.appuser).WithMany(x => x.PlayLists).HasForeignKey(x => x.UserId);
        }
    }
}
