using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Common.Exceptions;

namespace Common
{
    public abstract class BaseConsumer<T>(KafkaSettings settings, string topic, string groupId, ILogger logger) : BackgroundService
    {
        const int PollingDelay = 1000;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = groupId,
                AllowAutoCreateTopics = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config)
                .Build();

            consumer.Subscribe(topic);
            logger.LogInformation($"Kafka Consumer подписался на топик {topic}");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = await Task.Run(() => consumer.Consume(stoppingToken));

                        logger.LogInformation($"Получено сообщение. Key: {result.Message.Key}, Value: {result.Message.Value}");

                        T? _value = JsonSerializer.Deserialize<T>(result.Message.Value) 
                            ?? throw new DeserializeMessageException($"Ошибка десериализации сообщения: " + result.Message.Value);

                        // Бизнес логика
                        await ProcessMessageAsync(result.Message.Key, _value, stoppingToken);

                        consumer.Commit(result);
                    }
                    catch (ConsumeException ex) when (ex.Error.Reason.Contains("Subscribed topic not available"))
                    {
                        logger.LogInformation("Топик еще создается брокером. Повторная попытка через 2 секунды...");
                        await Task.Delay(2000);
                    }
                    catch (ConsumeException e)
                    {
                        logger.LogError("Ошибка Kafka при чтении: {Reason}", e.Error.Reason);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Критическая ошибка при обработке сообщения в бизнес-логике");
                    }
                    await Task.Delay(PollingDelay, stoppingToken);
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

        protected abstract Task ProcessMessageAsync(string key, T value, CancellationToken ct);

        public override void Dispose()
        {
            base.Dispose();
        }
    }


}
    