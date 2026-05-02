using BrainBurst.Application.DTOs;

namespace BrainBurst.Application.Interfaces.Services;

public interface IAuthContext
{
    int CurrentUserId { get; }
    UserDTO? CurrentUser { get; }
    void SetCurrentUser(UserDTO user);
    void ClearContext();
}