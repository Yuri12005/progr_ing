namespace BrainBurst.Domain.Entities;

/// <summary>
/// Представляє сутність "Тест", який об'єднує набір флеш-карток.
/// </summary>
public class Test
{
    public int TestId { get; set; }

    public string Title { get; set; } = string.Empty;

    // === НОВІ ПОЛЯ ===
    // Робимо Nullable (int?), бо раптом колись ви додасте тести з файлів, де колоди немає
    public int? TagId { get; set; }
    public Tag? Tag { get; set; }


    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;

    public ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}