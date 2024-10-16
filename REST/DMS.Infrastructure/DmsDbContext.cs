using System.Reflection.Metadata;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure;


public class DmsDbContext : DbContext
{
    public DbSet<DmsDocument> Documents { get; set; }
    public DbSet<DocumentTag> DocumentTags { get; set; }
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
        modelBuilder.Entity<DocumentTag>()
            .HasKey(dt => new { dt.DocumentId, dt.TagId });

        modelBuilder.Entity<DocumentTag>()
            .HasOne(dt => dt.Document)
            .WithMany(d => d.Tags)
            .HasForeignKey(dt => dt.DocumentId);

        modelBuilder.Entity<DocumentTag>()
            .HasOne(dt => dt.Tag)
            .WithMany()
            .HasForeignKey(dt => dt.TagId);
        
        base.OnModelCreating(modelBuilder);
    }
}
