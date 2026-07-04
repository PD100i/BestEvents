using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Bookings.Infrastructure
{
    /// <summary>
    /// Конфигурация сущности BookingEntity для EntityFrameworkCore
    /// </summary>
    public class BookingsConfiguration : IEntityTypeConfiguration<BookingEntity>
    {
        /// <summary>
        /// Конфигурирует сущность BookingEntity, задавая имя таблицы, ключи, свойства и связи
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<BookingEntity> builder)
        {
            builder.ToTable("bookings");
            builder.HasKey(b => b.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(b => b.EventId)
                .HasColumnName("event_id")
                .IsRequired();

            builder.Property(b => b.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(b => b.Status)
                .HasColumnName("status")
                .IsRequired();

            builder.Property(b => b.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(b => b.ProcessedAt)
                .HasColumnName("processed_at");

           

            
        }
    }
}