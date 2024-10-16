using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DMS.Infrastructure
{
    public class DmsDbContextFactory : IDesignTimeDbContextFactory<DmsDbContext>
    {
        public DmsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DmsDbContext>();

            optionsBuilder.UseNpgsql("Host=localhost;Database=DMSDB;Username=dmsadmin;Password=dmsadmin");

            return new DmsDbContext(optionsBuilder.Options);
        }
    }
}