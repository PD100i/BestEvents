using Microsoft.EntityFrameworkCore;

namespace Bookings.Infrastructure
{ 
    /// <summary>
    /// Конекст базы данных приложения
    /// </summary>
    /// <param name="options"></param>
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

        /// <summary>
        /// Таблица для хранения бронирований
        /// </summary>
        public DbSet<BookingEntity> Bookings { get; set; }

        public DbSet<BookingMessageEntity> BookingCreatedOutbox {  get; set; }


        public DbSet<BookingMessageEntity> BookingCancelledOutbox { get; set; }

        /// <summary>
        /// Применяет конфигурацию сущностей к модели данных
        /// </summary>
        /// <param name="modelBuilder">Объект ModelBuilder, используемый для настройки модели данных.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookingsConfiguration());
            modelBuilder.ApplyConfiguration(new BookingCreatedOutboxConfiguration());
        }
    }
}
