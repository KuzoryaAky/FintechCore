using FintechCore.Domain.Entities;
using FintechCore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace FintechCore.Infrastructure.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly FintechDbContext _context;

        public OutboxRepository(FintechDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Outbox outbox)
        {
            await _context.Outboxe.AddAsync(outbox);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Outbox>> GetUnprocessedAsync(int limit = 10)
        {
            return await _context.Outboxe
                .Where(o => !o.Processed)
                .OrderBy(o => o.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task MarkAsProcessedAsync(int id)
        {
            var outbox = await _context.Outboxe.FindAsync(id);
            if (outbox != null)
            {
                outbox.Processed = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
