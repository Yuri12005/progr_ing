using Microsoft.AspNetCore.Identity;

namespace BrainBurst.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
    public int Points { get; set; } = 0;
    public string Rank { get; set; } = "Beginner";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Додаємо навігаційні властивості, які були у твоєму класі User
    public virtual ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}