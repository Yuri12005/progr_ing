namespace BrainBurst.Application.DTOs;

public class UserDTO
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public int Points { get; set; }
    public string Rank { get; set; } = null!;
}