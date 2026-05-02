using BrainBurst.Domain.Entities;

namespace BrainBurst.Application.Interfaces.Repositories;

public interface ITestResultRepository
{
    Task<TestResult> AddAsync(TestResult tr, IEnumerable<QuestionResult> qr, CancellationToken ct);
    Task<IReadOnlyList<TestResult>> GetByUserAsync(int userId, CancellationToken ct);
}