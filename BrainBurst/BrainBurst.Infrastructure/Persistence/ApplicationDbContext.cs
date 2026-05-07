using BrainBurst.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BrainBurst.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Flashcard> Flashcards { get; set; } = null!;
    public DbSet<Test> Tests { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<TestResult> TestResults { get; set; } = null!;
    public DbSet<QuestionResult> QuestionResults { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Спочатку викликаємо базовий метод Identity
        base.OnModelCreating(modelBuilder);

        // 2. Налаштування нашої таблиці Users (колишній User)
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            // Identity використовує Id, але ми можемо налаштувати довжину полів
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Rank).HasMaxLength(20);
        });

        // 3. Твоя логіка Flashcard
        modelBuilder.Entity<Flashcard>(entity =>
        {
            entity.HasKey(e => e.FlashcardId);
            entity.HasOne(e => e.Creator)
                  .WithMany(u => u.Flashcards)
                  .HasForeignKey(e => e.CreatorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 4. Твоя логіка Tag (Many-to-Many)
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.Creator)
                  .WithMany(u => u.Tags)
                  .HasForeignKey(e => e.CreatorId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Flashcards)
                  .WithMany(f => f.Tags)
                  .UsingEntity(j => j.ToTable("FlashcardTags"));
        });

        // 5. Твоя логіка Test
        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.TestId);
            entity.HasOne(e => e.Creator)
                  .WithMany(u => u.Tests)
                  .HasForeignKey(e => e.CreatorId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 6. Твоя логіка TestResult
        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.TestResultId);
            entity.Property(e => e.CorrectAnswersPercent).HasColumnType("numeric(5,2)");
            
            entity.HasOne(e => e.Test)
                  .WithMany(t => t.TestResults)
                  .HasForeignKey(e => e.TestId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.User)
                  .WithMany(u => u.TestResults)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 7. Твоя логіка QuestionResult
        modelBuilder.Entity<QuestionResult>(entity =>
        {
            entity.HasKey(e => e.QuestionResultId);
            entity.HasOne(e => e.TestResult)
                  .WithMany(tr => tr.QuestionResults)
                  .HasForeignKey(e => e.TestResultId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Flashcard)
                  .WithMany(f => f.QuestionResults)
                  .HasForeignKey(e => e.FlashcardId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Перейменування інших системних таблиць Identity для краси
        modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
    }
}