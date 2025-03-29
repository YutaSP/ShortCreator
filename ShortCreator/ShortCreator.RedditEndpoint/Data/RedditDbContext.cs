using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShortCreator.RedditEndpoint.Models;

namespace ShortCreator.RedditEndpoint.Data;

public partial class RedditDbContext : DbContext
{
    public RedditDbContext()
    {
    }

    public RedditDbContext(DbContextOptions<RedditDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RedditStory> RedditStories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RedditStory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Reddit_Stories");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Story).IsUnicode(false);
            entity.Property(e => e.Title).IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
