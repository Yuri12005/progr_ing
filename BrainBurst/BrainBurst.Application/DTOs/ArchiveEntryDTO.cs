namespace BrainBurst.Application.DTOs;

public class ArchiveEntryDTO
{
    public int TestResultId { get; set; }
    public string Title { get; set; } = null!; // <--- ÄÎÄÀËÈ ÖÅÉ ÐßÄÎÊ
    public DateTime TestDate { get; set; }
    public decimal Score { get; set; }
    public int Points { get; set; }
}