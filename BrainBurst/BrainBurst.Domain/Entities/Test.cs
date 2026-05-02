namespace BrainBurst.Domain.Entities;

/// <summary>
/// Представляє сутність "Тест", який об'єднує набір флеш-карток.
/// </summary>
public class Test
{
    public int TestId { get; set; }
    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;

    public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}