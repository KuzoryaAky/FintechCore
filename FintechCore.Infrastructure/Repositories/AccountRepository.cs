using FintechCore.Domain.Entities;
using FintechCore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FintechCore.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        FintechDbContext _dbContext; 
        public AccountRepository(FintechDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Account account)
        {
            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

        }

        public async Task CompensateAsync(int accountId, decimal amount)
        {
            var account = await GetByIdAsync(accountId);
            if (account is not null)
            {
                account.Balance += amount;
                await UpdateAsync(account);
            }
        }

        public async Task DeleteAsync(Account account)
        {
            if (account is not null)
            {
                _dbContext.Accounts.Remove(account);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _dbContext.Accounts.ToListAsync();
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _dbContext.FindAsync<Account>(id);
        }

        public async Task UpdateAsync(Account account)
        {
            _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();
        }
    }
}
