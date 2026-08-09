using FintechCore.Domain.Entities;
using FintechCore.Domain.Interfaces;
using FintechCore.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Text;

namespace FintechCore.Workers
{
    public class OutboxPublisherService : BackgroundService
    {
        private readonly IOutboxRepository _outboxRepository;
        private readonly IChannel _channel;
        private readonly IConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;
        private bool _disposed;

        public OutboxPublisherService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            // Асинхронное объявление очереди
            _channel.QueueDeclareAsync(
                queue: "outbox_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            ).GetAwaiter().GetResult();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                    var events = await outboxRepository.GetUnprocessedAsync(10);

                    foreach (var outboxEvent in events)
                    {
                        try
                        {
                            var body = Encoding.UTF8.GetBytes(outboxEvent.Payload);

                            await _channel.BasicPublishAsync(
                                exchange: "",
                                routingKey: "outbox_queue",
                                body: body,
                                cancellationToken: stoppingToken
                            );

                            await outboxRepository.MarkAsProcessedAsync(outboxEvent.Id);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Outbox] Ошибка отправки: {ex.Message}");
                        }
                    }
                }

                await Task.Delay(5000, stoppingToken);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _channel?.CloseAsync().GetAwaiter().GetResult();
                _connection?.CloseAsync().GetAwaiter().GetResult();
            }

            _disposed = true;
        }

        
    }
}
