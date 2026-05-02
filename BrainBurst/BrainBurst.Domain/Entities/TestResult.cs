namespace BrainBurst.Domain.Entities;

/// <summary>
/// Представляє результат проходження тесту користувачем.
/// </summary>
public class TestResult
{
    public int TestResultId { get; set; }
    public int TestId { get; set; }
    public Test Test { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public decimal CorrectAnswersPercent { get; set; }
    public int Points { get; set; }
    public DateTime TestDate { get; set; } = DateTime.UtcNow;

    public ICollection<QuestionResult> QuestionResults { get; set; } = new List<QuestionResult>();
}