using System.Reflection.Metadata;
using DMS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure;


public class DmsDbContext : DbContext
{
    public DbSet<DocumentModel> DocumentModels { get; set; }
    public DbSet<DocumentTagModel> DocumentTagModels { get; set; }
    public DbSet<TagModel> TagModels { get; set; }

    public DmsDbContext(DbContextOptions<DmsDbContext> options) : base(options)
    {
        Console.WriteLine("DmsDbContext initialized.");
        Console.WriteLine(this.Database.CanConnect()
            ? "Connected to the database successfully."
            : "Failed to connect to the database.");
    }

    public void ApplyMigrations()
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentModel>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<DocumentModel>()
            .Property(e => e.Title).HasMaxLength(50);

        modelBuilder.Entity<TagModel>()
            .Property(e => e.Label)
            .HasMaxLength(20);

        modelBuilder.Entity<TagModel>()
            .Property(e => e.Value)
            .HasMaxLength(50);
        
        modelBuilder.Entity<TagModel>()
            .Property(e => e.Color)
            .HasMaxLength(8);
        
        modelBuilder.Entity<TagModel>()
            .HasIndex(e => e.Value)
            .IsUnique();
        
        modelBuilder.Entity<TagModel>()
            .HasIndex(e => e.Label)
            .IsUnique();

        modelBuilder.Entity<DocumentTagModel>()
            .HasKey(dt => new { dt.DocumentId, dt.TagId });
        
        modelBuilder.Entity<DocumentTagModel>()
            .HasOne(dt => dt.DocumentModels)
            .WithMany(d => d.Tags)
            .HasForeignKey(dt => dt.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DocumentTagModel>()
            .HasOne(dt => dt.TagModels)
            .WithMany(t => t.DocumentTags)
            .HasForeignKey(dt => dt.TagId);
        
        base.OnModelCreating(modelBuilder);
    }
}
