using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DMS.Infrastructure
{
    public class DmsDbContextFactory : IDesignTimeDbContextFactory<DmsDbContext>
    {
        public DmsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DmsDbContext>();
            IConfigurationRoot configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("dbSettings.json")
                .Build();
            
            var connectionString = configurationBuilder.GetConnectionString("DmsDbContext");
            optionsBuilder.UseNpgsql("Host=localhost;Database=DMSDB;Username=dmsadmin;Password=dmsadmin;Include Error Detail=true");
            return new DmsDbContext(optionsBuilder.Options);
        }
    }
}