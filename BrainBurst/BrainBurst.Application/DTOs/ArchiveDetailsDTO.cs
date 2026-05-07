namespace BrainBurst.Application.DTOs;

public class ArchiveDetailsDTO
{
    public string TestTitle { get; set; } = null!;
    public int PointsEarned { get; set; }
    public int MaxPoints { get; set; }
    public decimal ScorePercent { get; set; }
    public List<ArchiveQuestionDTO> Questions { get; set; } = new List<ArchiveQuestionDTO>();
}

public class ArchiveQuestionDTO
{
    public string QuestionText { get; set; } = null!;
    public string CorrectAnswer { get; set; } = null!;
    public string UserAnswer { get; set; } = null!;
    public bool IsCorrect { get; set; }
}