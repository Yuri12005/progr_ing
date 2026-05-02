namespace BrainBurst.Application.DTOs;

public class ArchiveEntryDTO
{
    public int TestResultId { get; set; }
    public DateTime TestDate { get; set; }
    public decimal Score { get; set; }
    public int Points { get; set; }
}