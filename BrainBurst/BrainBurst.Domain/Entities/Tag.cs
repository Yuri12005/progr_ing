namespace BrainBurst.Domain.Entities;

/// <summary>
/// Представляє сутність "Тег", яка використовується для категоризації флеш-карток.
/// </summary>
public class Tag
{
    public int TagId { get; set; }
    public string Name { get; set; } = null!;
    public int? CreatorId { get; set; }
    public ApplicationUser? Creator { get; set; }

    public ICollection<Flashcard> Flashcards { get; set; } = new HashSet<Flashcard>();
}