using BrainBurst.Domain.Enums;

namespace BrainBurst.Application.Interfaces.Services;

public interface IRatingService
{
    UserRank GetRank(int points);
    string GetRankLabel(UserRank rank);
}