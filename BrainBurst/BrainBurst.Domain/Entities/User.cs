namespace BrainBurst.Domain.Entities;

/// <summary>
/// Представляє сутність "Користувач".
/// </summary>
public class User
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? FullName { get; set; }
    public int Points { get; set; } = 0;
    public string Rank { get; set; } = "Початківець";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    public ICollection<Test> Tests { get; set; } = new List<Test>();
    public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}