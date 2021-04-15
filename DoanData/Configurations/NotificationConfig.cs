using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    public class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notification");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreateDate).HasDefaultValue<string>("00/00/00 00:0:00");
            builder.HasOne(x => x.appUser).WithMany(j => j.Notifications).HasForeignKey(s => s.UserId);
        }
    }
}
