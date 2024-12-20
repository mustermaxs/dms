﻿using System.Reflection.Metadata;
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
        modelBuilder.Entity<DmsDocument>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<DmsDocument>()
            .OwnsOne(e => e.DocumentType, dt =>
            {
                dt.Property(d => d.Name)
                    .HasColumnName("DocumentType")
                    .HasMaxLength(6);
            })
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

        modelBuilder.Entity<DocumentTag>()
            .HasKey(dt => new { dt.DocumentId, dt.TagId });

        modelBuilder.Entity<DocumentTag>()
            .HasOne(dt => dt.Document)
            .WithMany(d => d.Tags)
            .HasForeignKey(dt => dt.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DocumentTag>()
            .HasOne(dt => dt.Tag)
            .WithMany()
            .HasForeignKey(dt => dt.TagId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
