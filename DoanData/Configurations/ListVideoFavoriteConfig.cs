using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    public class ListVideoFavoriteConfig : IEntityTypeConfiguration<ListVideoFavavorite>
    {
        public void Configure(EntityTypeBuilder<ListVideoFavavorite> builder)
        {
            builder.ToTable("ListVideoFavorite");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.appUser).WithMany(j => j.ListVideoFavavorites).HasForeignKey(l => l.UserId);
            builder.Property(x => x.CreateDate).HasDefaultValue<string>("00/00/00 00:0:00");
        }
    }
}
