namespace BrainBurst.Application.Interfaces.Services;

public interface IQuizGenerator
{
    Task<IReadOnlyList<(string Question, string Answer)>> GenerateFromTextAsync(string text, CancellationToken ct);
}