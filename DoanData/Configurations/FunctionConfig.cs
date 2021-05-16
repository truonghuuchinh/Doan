using DoanData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoanData.Configurations
{
    public class FunctionConfig : IEntityTypeConfiguration<Function>
    {
        public void Configure(EntityTypeBuilder<Function> builder)
        {
            builder.ToTable("Function");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(250);
      
            builder.Property(x => x.Status).HasDefaultValue<bool>(true);
        }
    }
}
