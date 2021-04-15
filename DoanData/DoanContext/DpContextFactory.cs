using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DoanData.DoanContext
{
    public class DpContextFactory : IDesignTimeDbContextFactory<DpContext>
    {
        public DpContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile("appsettings.json").Build();
            var optionBuilder = new DbContextOptionsBuilder<DpContext>();
            optionBuilder.UseSqlServer(configurationRoot.GetConnectionString("DpContext"));
            return new DpContext(optionBuilder.Options);
        }
    }
}
