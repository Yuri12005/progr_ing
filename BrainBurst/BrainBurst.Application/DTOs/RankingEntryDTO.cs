namespace BrainBurst.Application.DTOs;

public class RankingEntryDTO
{
    public int RankPosition { get; set; }
    public string? FullName { get; set; }
    public int Points { get; set; }
    public string RankName { get; set; } = null!;
}