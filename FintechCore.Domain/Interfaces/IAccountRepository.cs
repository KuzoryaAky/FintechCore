using FintechCore.Domain.Entities;

namespace FintechCore.Domain.Interfaces
{
    public interface IAccountRepository
    {
        public Task<Account?> GetByIdAsync(int id);
        public Task AddAsync(Account account);
        public Task UpdateAsync(Account account);
        public Task DeleteAsync(Account account);
        public Task CompensateAsync(int accountId, decimal amount);
        public Task<IEnumerable<Account>> GetAllAsync();
    }
}
