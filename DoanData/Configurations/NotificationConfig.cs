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
            builder.Property(x => x.CreateDate).HasDefaultValue<string>(DateTime.Now.ToString("d-MM-yyyy H:mm:ss"));
            builder.Property(x => x.Watched).HasDefaultValue<bool>(true);
            builder.HasOne(x => x.appUser).WithMany(j => j.Notifications).HasForeignKey(s => s.UserId).HasPrincipalKey(t => t.Id); 
            builder.HasOne(x => x.fromAppUser).WithMany(x => x.FromNotifications).HasForeignKey(x => x.FromUserId).OnDelete(DeleteBehavior.NoAction).HasPrincipalKey(t => t.Id);
            builder.HasOne(x => x.video).WithMany(x => x.Notifications).HasForeignKey(x => x.VideoId).OnDelete(DeleteBehavior.NoAction).HasPrincipalKey(t => t.Id);

        }
    }
}
