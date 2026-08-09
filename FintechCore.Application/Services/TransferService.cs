using FintechCore.Domain.Entities;
using FintechCore.Domain.Interfaces;

namespace FintechCore.Application.Services
{
    public class TransferService : ITransferService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IOutboxRepository _outboxRepo;

        public TransferService(IAccountRepository accountRepo, ITransactionRepository transactionRepo, IOutboxRepository outboxRepo)
        {
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
            _outboxRepo = outboxRepo;
        }

        public async Task<TransferResult> TransferAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            // Проверки
            if (amount <= 0)
                return new TransferResult { Success = false, Message = "Сумма должна быть > 0" };
            if (fromAccountId == toAccountId)
                return new TransferResult { Success = false, Message = "Нельзя перевести на тот же счёт" };

            var from = await _accountRepo.GetByIdAsync(fromAccountId);
            var to = await _accountRepo.GetByIdAsync(toAccountId);
            if (from is null || to is null)
                return new TransferResult { Success = false, Message = "Счёт не найден" };
            if (from.Balance < amount)
                return new TransferResult { Success = false, Message = "Недостаточно средств" };

            // Обновление балансов
            from.Balance -= amount;
            to.Balance += amount;
            await _accountRepo.UpdateAsync(from);
            await _accountRepo.UpdateAsync(to);

            // Создание транзакции
            var transaction = new Transaction
            {
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Amount = amount,
                Status = "Completed",
                SagaId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };
            await _transactionRepo.AddAsync(transaction);

            // Запись в Outbox
            var outboxEvent = new Outbox
            {
                EventType = "MoneyTransferred",
                Payload = $"{{\"from\":{fromAccountId},\"to\":{toAccountId},\"amount\":{amount},\"timestamp\":\"{DateTime.UtcNow:O}\"}}",
                Processed = false,
                CreatedAt = DateTime.UtcNow
            };
            await _outboxRepo.AddAsync(outboxEvent);

            return new TransferResult
            {
                Success = true,
                TransactionId = transaction.Id,
                FromBalance = from.Balance,
                ToBalance = to.Balance
            };
        }
    }
}
