using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BrainBurst.Infrastructure.Persistence.Repositories;

public class TestResultRepository : ITestResultRepository
{
    private readonly ApplicationDbContext _context;

    public TestResultRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TestResult> AddAsync(TestResult tr, IEnumerable<QuestionResult> qr, CancellationToken ct)
    {
        // Додаємо загальний результат тесту
        await _context.TestResults.AddAsync(tr, ct);
        
        // Прив'язуємо кожну відповідь до цього результату і зберігаємо
        foreach (var questionResult in qr)
        {
            questionResult.TestResult = tr;
            await _context.QuestionResults.AddAsync(questionResult, ct);
        }

        await _context.SaveChangesAsync(ct);
        return tr;
    }

    public async Task<IReadOnlyList<TestResult>> GetByUserAsync(int userId, CancellationToken ct)
    {
        return await _context.TestResults
            .Include(tr => tr.Test)
            .Include(tr => tr.QuestionResults)
                .ThenInclude(qr => qr.Flashcard) // Підтягуємо самі картки, щоб бачити, на що відповідав користувач
            .Where(tr => tr.UserId == userId)
            .OrderByDescending(tr => tr.TestDate)
            .ToListAsync(ct);
    }
}