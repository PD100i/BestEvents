using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common
{
    public abstract class BaseConsumer<TKey, TValue>(string topic, string groupId, ILogger logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<TKey, TValue>(config)
                .Build();

            consumer.Subscribe(topic);
            logger.LogInformation($"Kafka Consumer подписался на топик {topic}");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(stoppingToken);

                        logger.LogInformation($"Получено сообщение. Key: {result.Message.Key}, Value: {result.Message.Value}");

                        // Бизнес логика
                        await ProcessMessageAsync(result.Message.Key, result.Message.Value, stoppingToken);

                        consumer.Commit(result);
                    }
                    catch (ConsumeException e)
                    {
                        logger.LogError("Ошибка Kafka при чтении: {Reason}", e.Error.Reason);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Критическая ошибка при обработке сообщения в бизнес-логике");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation($"Цикл Consumer остановлен через CancellationToken.");
            }
            finally
            {
                logger.LogInformation("Закрываем соединение с Kafka");
                consumer.Close();
            }
        }

        protected abstract Task ProcessMessageAsync(TKey key, TValue value, CancellationToken ct);

        public override void Dispose()
        {
            base.Dispose();
        }
    }


}
    