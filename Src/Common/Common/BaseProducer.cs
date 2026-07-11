using Confluent.Kafka;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common
{
    public class BaseProducer<T>
    {
        readonly ProducerConfig config;
        readonly IProducer<string, string> producer;
        readonly string topic;
        readonly ILogger logger;

        public BaseProducer(KafkaSettings settings, string topic, ILogger logger) 
        {
            config = new ProducerConfig()
            {
                BootstrapServers = settings.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true
            };
            producer = new ProducerBuilder<string, string>(config).Build();
            this.topic = topic;
            this.logger = logger;
        }

        public async Task PublicationAsync(string key, T value)
        {
            string _value = JsonSerializer.Serialize(value);
            var message = new Message<string, string>()
            {
                Key = key,
                Value = _value
            };
            var results = await producer.ProduceAsync(topic, message);
            logger.LogInformation($"Сообщение {results.Key} опубликовано в топик {results.Topic} в партицию {results.Partition}");

        }
    }
}
