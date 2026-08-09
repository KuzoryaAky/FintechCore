using FintechCore.Domain.Entities;
using FintechCore.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace FintechCore.Workers
{
    public class TransactionGeneratorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Random _random = new();

        public TransactionGeneratorService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                // Получаем сервисы через DI
                var accountRepo = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
                var transferService = scope.ServiceProvider.GetRequiredService<ITransferService>();

                // Проверяем, есть ли аккаунты, если нет — создаём 100
                var allAccounts = (await accountRepo.GetAllAsync()).ToList();

                if (allAccounts.Count < 100)
                {
                    Console.WriteLine($"[Генератор] Создаю {100 - allAccounts.Count} новых аккаунтов...");

                    for (int i = 0; i < 100 - allAccounts.Count; i++)
                    {
                        var newAccount = new Account
                        {
                            UserId = _random.Next(1, 1000),
                            Balance = _random.Next(100, 5000),
                            Status = "Active",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        await accountRepo.AddAsync(newAccount);
                    }

                    allAccounts = (await accountRepo.GetAllAsync()).ToList();
                    Console.WriteLine($"[Генератор] Теперь в системе {allAccounts.Count} аккаунтов.");
                }

                // --- Генерация случайного перевода через TransferService ---
                if (allAccounts.Count >= 2)
                {
                    var from = allAccounts[_random.Next(allAccounts.Count)];
                    var to = allAccounts[_random.Next(allAccounts.Count)];

                    if (from.Id != to.Id && from.Balance >= 10)
                    {
                        var amount = _random.Next(10, 501);

                        // Вызываем TransferService — он сам создаст транзакцию и Outbox
                        var result = await transferService.TransferAsync(from.Id, to.Id, amount);

                        if (result.Success)
                        {
                            Console.WriteLine($"[Генератор] Перевод {amount} руб. от {from.Id} к {to.Id} (баланс отправителя: {result.FromBalance})");
                        }
                        else
                        {
                            Console.WriteLine($"[Генератор] Ошибка: {result.Message}");
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
