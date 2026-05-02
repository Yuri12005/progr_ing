using BrainBurst.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrainBurst.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Таблиці нашої бази даних
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Flashcard> Flashcards { get; set; } = null!;
    public DbSet<Test> Tests { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<TestResult> TestResults { get; set; } = null!;
    public DbSet<QuestionResult> QuestionResults { get; set; } = null!;

    // Налаштування правил таблиць (замість старих [Key], [MaxLength] і т.д.)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Налаштування User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Rank).HasMaxLength(20);
        });

        // Налаштування Flashcard
        modelBuilder.Entity<Flashcard>(entity =>
        {
            entity.HasKey(e => e.FlashcardId);
            
            // Зв'язок: Один користувач -> Багато карток
            entity.HasOne(e => e.Creator)
                  .WithMany(u => u.Flashcards)
                  .HasForeignKey(e => e.CreatorId)
                  .OnDelete(DeleteBehavior.Restrict); // Забороняємо каскадне видалення користувача при видаленні картки
        });

        // Налаштування Tag (та зв'язок Many-to-Many з Flashcards)
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.Creator)
                  .WithMany(u => u.Tags)
                  .HasForeignKey(e => e.CreatorId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Багато-до-багатьох: Картка <-> Тег
            entity.HasMany(e => e.Flashcards)
                  .WithMany(f => f.Tags)
                  .UsingEntity(j => j.ToTable("FlashcardTags")); // EF Core автоматично створить проміжну таблицю!
        });

        // Налаштування Test
        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.TestId);
            entity.HasOne(e => e.Creator)
                  .WithMany(u => u.Tests)
                  .HasForeignKey(e => e.CreatorId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Налаштування TestResult
        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.TestResultId);
            // Тут ми переносимо твоє старе правило [Column(TypeName = "numeric(5,2)")]
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

        // Налаштування QuestionResult
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
    }
}