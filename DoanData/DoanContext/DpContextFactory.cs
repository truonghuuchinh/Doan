using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DoanData.DoanContext
{
    public class DpContextFactory : IDesignTimeDbContextFactory<DpContext>
    {
        public DpContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var optionBuilder = new DbContextOptionsBuilder<DpContext>();
            optionBuilder.UseSqlServer(configurationRoot.GetConnectionString("SocialDb"));
            return new DpContext(optionBuilder.Options);
        }
    }
}
