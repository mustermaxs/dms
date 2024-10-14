using System.Reflection.Metadata;
using DMS.Domain.Entities;
using DMS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure;


public class DmsDbContext : DbContext
{
    public DbSet<DmsDocument> Documents { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public DmsDbContext(DbContextOptions<DmsDbContext> options) : base(options)
    {
        Console.WriteLine("DmsDbContext initialized.");
        Console.WriteLine(this.Database.CanConnect()
            ? "Connected to the database successfully."
            : "Failed to connect to the database.");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DmsDocument>()
            .HasMany<Tag>(e => e.Tags)
            .WithMany(t => t.Documents)
            .UsingEntity(j => j.ToTable("DocumentTags"));

        base.OnModelCreating(modelBuilder);
        
        Console.WriteLine("Connected to db");
    }
}
