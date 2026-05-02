using BrainBurst.Application.DTOs;

namespace BrainBurst.Application.Interfaces.Services;

public interface IAuthService
{
    Task<UserDTO> RegisterAsync(string email, string password, string fullName, CancellationToken ct);
    Task<UserDTO> LoginAsync(string email, string password, CancellationToken ct);
}