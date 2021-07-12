using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    public class MessageConfig : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Message");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.AppusersSender).WithMany(x => x.MessagesSender).HasForeignKey(x => x.SenderId);
            builder.HasOne(x => x.AppusersReceiver).WithMany(x => x.MessagesReciever).HasForeignKey(x => x.ReceiverId).OnDelete(DeleteBehavior.NoAction).HasPrincipalKey(t => t.Id);
        }
    }
}
