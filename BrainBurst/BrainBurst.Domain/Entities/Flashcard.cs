namespace BrainBurst.Domain.Entities;

/// <summary>
/// Представляє сутність "Флеш-картка".
/// </summary>
public class Flashcard
{
    public int FlashcardId { get; set; }
    public string Question { get; set; } = null!;
    public string Answer { get; set; } = null!;
    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
    public ICollection<QuestionResult> QuestionResults { get; set; } = new HashSet<QuestionResult>();
}