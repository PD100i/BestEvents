using Events.Infrastructure.AppDb.Configurations;
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
        /// Инбокс таблица для сообщений о создании события
        /// </summary>
        public DbSet<BookingMessageEntity> BookingCreatedInbox { get; set; }

        /// <summary>
        /// Инбокс таблица для сообщений об отмене события
        /// </summary>
        public DbSet<BookingMessageEntity> BookingCancelledInbox { get; set; }

        /// <summary>
        /// Оутбокс таблица для сообщений об успешном резервировании
        /// </summary>
        public DbSet<BookingMessageEntity> SeatsReservedOutbox { get; set; }

        /// <summary>
        /// Оутбокс таблица для сообщений об ошибках резервирования
        /// </summary>
        public DbSet<BookingMessageEntity> SeatsReservationErrorOutbox { get; set; }

        /// <summary>
        /// Применяет конфигурацию сущностей к модели данных
        /// </summary>
        /// <param name="modelBuilder">Объект ModelBuilder, используемый для настройки модели данных.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new BookingCreatedInboxConfiguration());
            modelBuilder.ApplyConfiguration(new BookingCancelledInboxConfigutation());
            modelBuilder.ApplyConfiguration(new SeatsReservedOutboxConfiguration());
            modelBuilder.ApplyConfiguration(new SeatsReservationErrorOutboxConfiguration());
        }
    }
}
