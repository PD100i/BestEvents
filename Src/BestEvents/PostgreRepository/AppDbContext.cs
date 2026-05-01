using Microsoft.EntityFrameworkCore;

namespace BestEvents 
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
        /// Таблица для хранения бронирований
        /// </summary>
        public DbSet<BookingEntity> Bookings { get; set; }
    }
}
