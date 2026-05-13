using BrainBurst.Application.Interfaces.Repositories;
using BrainBurst.Domain.Entities;
using BrainBurst.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BrainBurst.Infrastructure.Persistence.Repositories;

public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _context;
    public TagRepository(ApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<Tag>> GetAllWithCardsAsync(int userId, CancellationToken ct)
    {
        return await _context.Tags
            // Шукаємо лише ті колоди (теги), в яких є картки поточного юзера
            .Where(t => t.Flashcards.Any(f => f.CreatorId == userId))
            // Підтягуємо лише картки цього юзера, щоб не рахувати чужі
            .Include(t => t.Flashcards.Where(f => f.CreatorId == userId))
            .ToListAsync(ct);
    }

    public async Task<Tag?> GetByIdWithCardsAsync(int id, CancellationToken ct)
    {
        return await _context.Tags
            .Include(t => t.Flashcards)
            .FirstOrDefaultAsync(t => t.TagId == id, ct);
    }
}