using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure
{
    public class RedisSettings
    {
        /// <summary>
        /// Коллекция конечных точек Redis-сервера. По умолчанию используется "localhost:6379".
        /// </summary>
        public EndPointCollection EndPoints { get; set; } = new EndPointCollection { "localhost:6379" };


        /// <summary>
        /// Пароль для подключения к Redis-серверу. По умолчанию пустая строка.
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// Время ожидания подключения к Redis-серверу в миллисекундах. По умолчанию 5000.
        /// </summary>
        public int ConnectTimeout { get; set; } = 5000;

        /// <summary>
        /// Время ожидания синхронных операций с Redis-сервером в миллисекундах. По умолчанию 3000.
        /// </summary>
        public int SyncTimeout { get; set; } = 3000;

        /// <summary>
        /// Определяет, следует ли прерывать попытки подключения при сбое. По умолчанию false.
        /// </summary>
        public bool AbortOnConnectFail { get; set; } = false;

        /// <summary>
        /// Time-to-live (TTL) для кэширования событий в секундах. По умолчанию 3 секунды.
        /// </summary>
        public int GetEvntTTL_sec { get; set; } = 3;

        /// <summary>
        /// Time-to-live (TTL) для кэширования топовых событий в секундах. По умолчанию 60 секунд.
        /// </summary>  
        public int GetTopEvntsTTL_sec { get; set; } = 60;
    }
}
