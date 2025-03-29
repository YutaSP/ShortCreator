using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShortCreator.YoutubeEndpoint.Models;

namespace ShortCreator.YoutubeEndpoint.Data;

public partial class YoutubeDbContext : DbContext
{
    public YoutubeDbContext()
    {
    }

    public YoutubeDbContext(DbContextOptions<YoutubeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<YoutubeTargetVidId> YoutubeTargetVidIds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
