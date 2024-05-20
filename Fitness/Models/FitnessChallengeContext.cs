using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Models;

public partial class FitnessChallengeContext : DbContext
{
    public FitnessChallengeContext()
    {
    }

    public FitnessChallengeContext(DbContextOptions<FitnessChallengeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Challenge> Challenges { get; set; }

    public virtual DbSet<ChallengeParticipants> ChallengeParticipants { get; set; }

    public virtual DbSet<Leaderboard> Leaderboards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = WebApplication.CreateBuilder();
        var connectionString = builder.Configuration.GetConnectionString ("MyConnection");
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.HasKey(e => e.ChallengeId).HasName("PK__chal__A0C649523BA50520");

            entity.ToTable("challenges");

            entity.Property(e => e.ChallengeId).HasColumnName("challengeId");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("endDate");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("startDate");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("title");
        });

        modelBuilder.Entity<ChallengeParticipants>(entity =>
        {
            entity.HasKey(e => e.ParticipantId).HasName("PK__chal__4EE79210DC8DA1C5");

            entity.ToTable("challengeParticipants");

            entity.Property(e => e.ParticipantId).HasColumnName("participantId");
            entity.Property(e => e.ChallengeId).HasColumnName("challengeId");
            entity.Property(e => e.JoinDate)
                .HasColumnType("datetime")
                .HasColumnName("joinDate");
            entity.Property(e => e.Progress)
                .HasMaxLength(100)
                .HasColumnName("progress");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Challenge).WithMany(p => p.ChallengeParticipants)
                .HasForeignKey(d => d.ChallengeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__chall__chall__4E88ABD4");
        });

        modelBuilder.Entity<Leaderboard>(entity =>
        {
            entity.HasKey(e => e.LeaderboardId).HasName("PK__lead__3B9417B5E438283C");

            entity.ToTable("leaderboard");

            entity.Property(e => e.LeaderboardId).HasColumnName("leaderboardId");
            entity.Property(e => e.ChallengeId).HasColumnName("challengeId");
            entity.Property(e => e.Rank).HasColumnName("rank");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("userId");

            entity.HasOne(d => d.Challenge).WithMany(p => p.Leaderboard)
                .HasForeignKey(d => d.ChallengeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__leade__chall__5165187F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
