using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShortCreator.RedditEndpoint.Models;

namespace ShortCreator.RedditEndpoint.data;

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

    public virtual DbSet<RedditUsedStory> RedditUsedStories { get; set; }

    public virtual DbSet<YoutubeTargetVidId> YoutubeTargetVidIds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost,1433;Initial Catalog=Short_Maker_Prod;User ID=sa;Password=!Test123;Trust Server Certificate=True");

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

        modelBuilder.Entity<RedditUsedStory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Reddit_Used_Stories");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<YoutubeTargetVidId>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Youtube_Target_Vid_Ids");

            entity.Property(e => e.RedditId).IsUnicode(false);
            entity.Property(e => e.VidPath)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
