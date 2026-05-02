namespace BrainBurst.Application.DTOs;

public class FlashcardDTO
{
    public int FlashcardId { get; set; }
    public string Question { get; set; } = null!;
    public string Answer { get; set; } = null!;
    public IEnumerable<string> Tags { get; set; } = new List<string>();
}