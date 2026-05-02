namespace BrainBurst.Application.DTOs;

public class TestDTO
{
    public int TestId { get; set; }
    public int CreatorId { get; set; }
    public IEnumerable<FlashcardDTO> Flashcards { get; set; } = new List<FlashcardDTO>();
}