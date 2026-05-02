namespace BrainBurst.Domain.Entities;

/// <summary>
/// Представляє результат відповіді користувача на конкретну флеш-картку під час тесту.
/// </summary>
public class QuestionResult
{
    public int QuestionResultId { get; set; }
    public int TestResultId { get; set; }
    public TestResult TestResult { get; set; } = null!;
    public int FlashcardId { get; set; }
    public Flashcard Flashcard { get; set; } = null!;
    public string UserInput { get; set; } = null!;
    public bool IsCorrect { get; set; }
}