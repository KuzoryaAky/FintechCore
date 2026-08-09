using FintechCore.Domain.Entities;

namespace FintechCore.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(int accountId);
        Task UpdateAsync(Transaction transaction);
    }
}
