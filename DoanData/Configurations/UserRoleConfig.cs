using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    public class UserRoleConfig : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRole");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreateDate).HasDefaultValue<string>("00/00/00 00:0:00");
            builder.HasOne(x => x.appUser).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.action).WithMany(x => x.UserRoles).HasForeignKey(x => x.ActionId);
        }
    }
}
