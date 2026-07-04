using Confluent.Kafka;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class BaseProducer<TKey, TValue>
    {
        readonly ProducerConfig config;
        readonly IProducer<TKey, TValue> producer;
        readonly string topic;
        readonly ILogger logger;

        public BaseProducer(string topic, ILogger logger) 
        {
            config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092",
                Acks = Acks.All,
                EnableIdempotence = true
            };
            producer = new ProducerBuilder<TKey, TValue>(config).Build();
            this.topic = topic;
            this.logger = logger;
        }

        public async Task PublicationAsync(TKey key, TValue value)
        {
            var message = new Message<TKey, TValue>()
            {
                Key = key,
                Value = value
            };
            var results = await producer.ProduceAsync(topic, message);
            logger.LogInformation($"Сообщение {results.Key} опубликовано в топик {results.Topic} в партицию {results.Partition}");

        }
    }
}
