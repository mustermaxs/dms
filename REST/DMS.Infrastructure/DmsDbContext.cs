using System.Reflection.Metadata;
using DMS.Domain.Entities;
using DMS.Domain.Entities.Tag;
using DMS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DMS.Infrastructure;


public class DmsDbContext : DbContext
{
    public DbSet<DmsDocument> Documents { get; set; }
    public DbSet<DocumentTag> DocumentTags { get; set; }
    public DbSet<Tag> Tags { get; set; }
    private readonly ILogger<DmsDbContext> _logger;


    public DmsDbContext(DbContextOptions<DmsDbContext> options, ILogger<DmsDbContext> logger) : base(options)
    {
        _logger = logger;
        
        var canConnect = this.Database.CanConnect();
        if (!canConnect)
        {
            _logger.LogError("Failed to connect to the database");
        }
    }

    public void ApplyMigrations()
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DmsDocument>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<DmsDocument>()
            .Property(e => e.Title).HasMaxLength(50);

        modelBuilder.Entity<Tag>()
            .Property(e => e.Label)
            .HasMaxLength(20);

        modelBuilder.Entity<Tag>()
            .Property(e => e.Value)
            .HasMaxLength(50);
        
        modelBuilder.Entity<Tag>()
            .Property(e => e.Color)
            .HasMaxLength(8);
        
        modelBuilder.Entity<Tag>()
            .HasIndex(e => e.Value)
            .IsUnique();
        
        modelBuilder.Entity<Tag>()
            .HasIndex(e => e.Label)
            .IsUnique();

        modelBuilder.Entity<DocumentTag>()
            .HasKey(dt => new { dt.DocumentId, dt.TagId });
        
        modelBuilder.Entity<DocumentTag>()
            .HasOne(dt => dt.Document)
            .WithMany(d => d.Tags)
            .HasForeignKey(dt => dt.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DocumentTag>()
            .HasOne(dt => dt.Tag)
            .WithMany(t => t.DocumentTags)
            .HasForeignKey(dt => dt.TagId);

        base.OnModelCreating(modelBuilder);
        
         Database.Migrate();
    }
}
