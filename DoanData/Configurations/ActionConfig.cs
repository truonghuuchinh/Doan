using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using DoanData.Models;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    public class ActionConfig : IEntityTypeConfiguration<Models.Action>
    {
        public void Configure(EntityTypeBuilder<Models.Action> builder)
        {
            builder.ToTable("Action");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
            builder.HasOne(x => x.function).WithMany(p => p.ActionList).HasForeignKey(k => k.FunctionsId);
        }
    }
}
