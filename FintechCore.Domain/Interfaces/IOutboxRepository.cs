using FintechCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FintechCore.Domain.Interfaces
{
    public interface IOutboxRepository
    {
        Task AddAsync(Outbox outbox);
        Task<IEnumerable<Outbox>> GetUnprocessedAsync(int limit = 10);
        Task MarkAsProcessedAsync(int id);
    }
}
