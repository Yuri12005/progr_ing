namespace BrainBurst.Application.DTOs;

public class TestResultDTO
{
    public int TestResultId { get; set; }
    public int TestId { get; set; }
    public int UserId { get; set; }
    public decimal CorrectAnswersPercent { get; set; }
    public int PointsEarned { get; set; }
    public DateTime TestDate { get; set; }
}