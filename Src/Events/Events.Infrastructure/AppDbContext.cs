using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure
{ 
    /// <summary>
    /// Конекст базы данных приложения
    /// </summary>
    /// <param name="options"></param>
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        /// <summary>
        /// Таблица для хранения событий
        /// </summary>
        public DbSet<EventEntity> Events { get; set; } 


        /// <summary>
        /// Инбокс таблица для сообщений
        /// </summary>
        public DbSet<MessageEntity> Inbox { get; set; }

        /// <summary>
        /// Оутбокс таблица для сообщений
        /// </summary>
        public DbSet<MessageEntity> Outbox { get; set; }

        /// <summary>
        /// Применяет конфигурацию сущностей к модели данных
        /// </summary>
        /// <param name="modelBuilder">Объект ModelBuilder, используемый для настройки модели данных.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new InboxConfigutation());
            modelBuilder.ApplyConfiguration(new OutboxConfiguration());
        }
    }
}
